#include <string.h>
#include <unistd.h>
#include <stdlib.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <stdio.h>
#include <mysql.h>
#include <pthread.h>

typedef struct{
	char usuario[20];
	int socket;
	
} Conectado;

typedef struct{
	Conectado usuarios[300];
	int num;
} ListaConectados;

ListaConectados UsuariosConectados;

//Estructura necesaria para acceso excluyente
pthread_mutex_t mutex = PTHREAD_MUTEX_INITIALIZER;

int i;
int sockets[100];
char notificacion[512];

int Add (ListaConectados *lista, char nombre[20])
//Añade el usuario que se a conectado a la lista
{
	
	if (lista->num == 300)
		return -1;
	else {
		strcpy(lista->usuarios[lista->num].usuario, nombre);
		lista->num++;
		return 0;
		
	}
	
}


void DameConectados (ListaConectados *lista, char conectados[512])
{
	sprintf(conectados,"6/%s", lista->usuarios[0].usuario);
	for(int j=1; j< lista->num; j++)
		sprintf(conectados,"%s, %s", conectados, lista->usuarios[j].usuario);
	//for(int k=0; k<i; k++)
		//write (sockets[k], conectados, strlen(conectados));
		
}

int DesconectarUsuario(ListaConectados *lista, char nombre[20])
{
	int j=0;
	int encontrado=0;
	while((j<lista->num) && !encontrado){
		if (strcmp(lista->usuarios[j].usuario,nombre)==0)
			encontrado=1;
		if(!encontrado)
			j=j+1;
	}
	if (encontrado)
	{
		for(int pos=j; pos<lista->num-1; pos++)
			strcpy(lista->usuarios[pos].usuario,lista->usuarios[pos+1].usuario);
		lista->num--;
		return 0;
	}
	
	else
		return -1;
		
	
}

void *AtenderCliente (void *socket)
{
		
	int sock_conn;
	int *s;
	s= (int *) socket;
	sock_conn= *s;
	
	MYSQL *conn;
	int err;
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	char consulta [80];
	conn = mysql_init(NULL);
	
	char peticion[512];
	char respuesta[512];
	int ret;
	
	if (conn==NULL) 
	{
		printf ("Error al crear la conexion: %u %s\n",mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	
	conn = mysql_real_connect (conn, "localhost","root", "mysql", "partidas",0, NULL, 0);
	if (conn==NULL) 
	{
		printf ("Error al inicializar la conexion: %u %s\n",mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	int terminar=0;
	while (terminar==0)
	{
		// Ahora recibimos su nombre, que dejamos en buff
		ret=read(sock_conn,peticion, sizeof(peticion));
		printf ("Recibido\n");
		
		// Tenemos que a?adirle la marca de fin de string 
		// para que no escriba lo que hay despues en el buffer
		peticion[ret]='\0';
		
		//Escribimos el nombre en la consola
		
		printf ("Se ha conectado: %s\n",peticion);
		
		char *p = strtok( peticion, "/");
		int codigo =  atoi (p);
		printf("codigo= %d\n",codigo);
		
		if (codigo==0)
		{
			char usuario[20];
			p=strtok(NULL, "/");
			strcpy(usuario, p);
			pthread_mutex_lock( &mutex );
			int eliminar = DesconectarUsuario(&UsuariosConectados, usuario);
			pthread_mutex_unlock( &mutex );
			char conectados[300];
			DameConectados(&UsuariosConectados, conectados); 
			sprintf(notificacion,"%s", conectados);
			for(int k=0; k<i; k++)
				write (sockets[k], notificacion, strlen(notificacion));
			if (eliminar==0)
				printf("El usuario se ha eliminado de la lista de conectados\n");
			else
				printf("El usuario no se ha podido eliminar de la lista de conectados\n");
			terminar=1;
		}
		
		if(codigo==1)
		{
			char usuario[20];
			char password[20];
			p=strtok(NULL, "/");
			strcpy(usuario, p);
			p=strtok(NULL, "/");
			strcpy(password, p);
			
			err=mysql_query (conn, "SELECT * FROM datos_jugador");
			if (err!=0) 
			{
				printf ("Error al consultar datos de la base %u %s\n",mysql_errno(conn), mysql_error(conn));
				exit (1);
			}
			resultado = mysql_store_result (conn);
			row = mysql_fetch_row (resultado);
			if (row == NULL)
				printf ("No se han obtenido datos en la consulta\n");
			else
			{
				while (row != NULL) 
				{
					if (strcmp (row[0],usuario) ==0)
					{
						if(strcmp(row[1],password)==0)
						{
							strcpy(respuesta,"1/1");
							
							pthread_mutex_lock( &mutex ); //No me interrumpe ahora
							int res = Add(&UsuariosConectados,usuario);
							pthread_mutex_unlock(&mutex);
							sprintf(notificacion,"6/%s",UsuariosConectados.usuarios[0].usuario);
							for(int j=1; j< UsuariosConectados.num; j++)
								sprintf(notificacion,"%s, %s", notificacion, UsuariosConectados.usuarios[j].usuario);
							for(int k=0; k<i; k++)
								write (sockets[k], notificacion, strlen(notificacion));
							if (res==0)
							{
								printf("El usuario %s se ha añadido a la lista de conectados \n", usuario );
							}
							else
								printf("La lista de usuarios esta llena");
						}
						else 
						{
							strcpy(respuesta,"1/0");
						}
						row = NULL;
					}
					else 
					{
						row = mysql_fetch_row (resultado);
					}	
				}
			}
						
		}
		
		if (codigo==2)
		{
			char usuario[20];
			char password[20];
			int edad;
			p=strtok(NULL, "/");
			strcpy(usuario, p);
			p=strtok(NULL, "/");
			strcpy(password, p);
			p=strtok(NULL, "/");
			edad= atoi(p);
			
			err=mysql_query (conn, "SELECT * FROM datos_jugador");
			if (err!=0) 
			{
				printf ("Error al consultar datos de la base %u %s\n",mysql_errno(conn), mysql_error(conn));
				exit (1);
			}
			resultado = mysql_store_result (conn);
			row = mysql_fetch_row (resultado);
			if (row == NULL)
				printf ("No se han obtenido datos en la consulta\n");
			else
			{
				char name[strlen(row[0])];
				strcpy(name,row[0]);
				while (row !=NULL) 
				{
					strcpy(respuesta, "2/1");
					if (strcmp (name,usuario) ==0)
					{
						strcpy(respuesta,"2/0");
					}
					else row = mysql_fetch_row (resultado);
				}
			}
			if (strcmp(respuesta,"2/1")==0)
			{
				char edads[3];
				strcpy(consulta, "INSERT INTO datos_jugador VALUES('");
				strcat(consulta, usuario);
				strcat (consulta, "','");
				strcat(consulta, password);
				strcat (consulta, "',");
				sprintf(edads, "%d", edad);
				strcat (consulta, edads);
				strcat(consulta, ");");
				//printf(consulta);
				err=mysql_query(conn, consulta);
				if (err!=0)
				{
					printf ("Error al introducir datos de la base %u %s \n", mysql_errno(conn), mysql_error(conn));
					exit(1);
				}
			}
			
		}
		
		if (codigo==3)
		{
			int Npartida;
			p=strtok(NULL, "/");
			Npartida=atoi(p);
			//Pido al usuario que introduzca el nombre de parti
			//Realizamos la consulta
			strcpy(consulta,"SELECT * FROM partida");
			err=mysql_query(conn,consulta);
			//err=mysql_query(conn,"SELECT relacion.jugador FROM relacion WHERE relacion.partida = 'p1'");
			if (err!=0)
			{
				printf("Error al realizar la consulta %u %s \n", mysql_errno(conn), mysql_error(conn));
				exit(1);
			}
			resultado=mysql_store_result(conn);
			row = mysql_fetch_row(resultado);
			
			if (row == NULL)
				strcpy(respuesta,"3/Base de datos vacia");
			else
			{
				strcpy(respuesta,"3/Los jugadores son: ");
				while (row!=NULL)
				{
					int cont = atoi(row[1]);
					if (cont==Npartida)
					{
						strcat (respuesta, row[0]);
						strcat (respuesta, " ");
					}
					row = mysql_fetch_row(resultado);
				}
				if (strcmp(respuesta,"3/Los jugadores son: ")==0)
					strcpy(respuesta,"3/Esta ID no corresponde a ninguna partida (prueba 1 o 2)");
				
				
			}	
			respuesta [strlen(respuesta)-1]='\0';
		}
		
		
		
		if(codigo==4)
		{
			err=mysql_query (conn, "SELECT * FROM datos_partida");
			if (err!=0) 
			{
				printf ("Error al consultar datos de la base %u %s\n",mysql_errno(conn), mysql_error(conn));
				exit (1);
			}
			
			
			resultado = mysql_store_result (conn);
			row = mysql_fetch_row (resultado);
			if (row == NULL)
				strcpy(respuesta, "4/Base de datos vacia");
			else
			{
				strcpy (respuesta, "4/Los ganadores son :");
				while (row !=NULL) 
				{
					strcat (respuesta, row[4]); 
					strcat (respuesta, " ");
					row = mysql_fetch_row (resultado);
				}
				
			}
			
			respuesta [strlen(respuesta)-1]='\0';
			
			
		}
		
		if (codigo==5)
		{
			int Npartida;
			p=strtok(NULL, "/");
			Npartida=atoi(p);
			char np[20];
			
			
			strcpy (consulta,"SELECT * FROM datos_partida WHERE ID = " );
			sprintf(np, "%d", Npartida);
			strcat (consulta, np);
			strcat (consulta, ";");
			err=mysql_query (conn,consulta );
			if (err!=0) 
			{
				printf ("Error al consultar datos de la base %u %s\n",mysql_errno(conn), mysql_error(conn));
				exit (1);
			}
			
			resultado = mysql_store_result (conn);
			row = mysql_fetch_row (resultado);
			if (row == NULL)
				strcpy(respuesta, "5/Base de datos vacia");
			else
			{
				
				char fecha[20];
				char hf[20];
				char dur[20];
				char nme[20];
				
				strcpy(fecha, row[1]);
				strcpy(hf,row[2]);
				strcpy(nme,row[4]);
				sprintf(respuesta, "5/ID: %d, Fecha: %s, Hora final: %s, Duracion: %dmin, Ganador: %s", Npartida,fecha,hf,atoi(row[3]),nme);
			}
		}
		if (codigo==6)
		{
			DameConectados(&UsuariosConectados, respuesta);
					
		}
		
		if (codigo !=0)
		{
			printf ("Respuesta: %s\n", respuesta);
			// Enviamos la respuesta
			write (sock_conn,respuesta, strlen(respuesta));
		}
		
		respuesta [strlen(respuesta)-1]='\0';
		
	}
	close(sock_conn);
	mysql_close (conn);
	
}

int main(int argc, char *argv[])
{	
	UsuariosConectados.num=0;
	int sock_conn, sock_listen;
	struct sockaddr_in serv_adr;
	
	// INICIALITZACIONS
	// Obrim el socket
	if ((sock_listen = socket(AF_INET, SOCK_STREAM, 0)) < 0)
		printf("Error creant socket");
	// Fem el bind al port
		
	memset(&serv_adr, 0, sizeof(serv_adr));// inicialitza a zero serv_addr
	serv_adr.sin_family = AF_INET;
	
	// asocia el socket a cualquiera de las IP de la m?quina. 
	//htonl formatea el numero que recibe al formato necesario
	serv_adr.sin_addr.s_addr = htonl(INADDR_ANY);
	// escucharemos en el port 9050
	serv_adr.sin_port = htons(9050);
	if (bind(sock_listen, (struct sockaddr *) &serv_adr, sizeof(serv_adr)) < 0)
		printf ("Error al bind\n");
	//La cola de peticiones pendientes no podr? ser superior a 4
	if (listen(sock_listen, 2) < 0)
		printf("Error en el Listen");
	
	
	pthread_t thread[100];
	i=0;
	for(;;)
	{
		printf ("Escuchando\n");
		
		sock_conn = accept(sock_listen, NULL, NULL);
		printf ("He recibido conexion \n");
		
		sockets[i] =sock_conn;
		//sock_conn es el socket que usaremos para este cliente
		
		// Crear thead y decirle lo que tiene que hacer	
		pthread_create (&thread[i], NULL, AtenderCliente,&sockets[i]);
		i=i+1;

	}
}
