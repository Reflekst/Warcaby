#include <netdb.h>
#include <pthread.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <unistd.h>
#include <arpa/inet.h>
#include <netinet/in.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <sys/wait.h>
#define MAX_LENGTH 5
#define END_CHAR "\n"

struct cln
{
    int cfd;
    struct sockaddr_in caddr;
};

struct clns
{
    struct cln *firstClient;
    struct cln *secondClient;
};

struct cln *lobby[2];

int readMessage(int cfd, char buffer[], int messageSize)
{
    int receivedBytes = 0;
    while (receivedBytes < messageSize)
    {
        int readValue = read(cfd, buffer + receivedBytes, messageSize - receivedBytes);
        if (readValue == -1 || readValue == 0)
        {
            return 1;
        }
        receivedBytes = receivedBytes + readValue;
    }
    return 0;
}

int writeMessage(int cfd, char buffer[], int messageSize)
{
    int sentBytes = 0;
    while (sentBytes < messageSize)
    {
        int writeValue = write(cfd, buffer + sentBytes, messageSize - sentBytes);
        if (writeValue == -1 || writeValue == 0)
        {
            return 1;
        }
        sentBytes = sentBytes + writeValue;
    }
    return 0;
}
void *serverThread(void *arg)
{

    struct clns *clients = arg;
    struct cln *firstClient = clients->firstClient;
    struct cln *secondClient = clients->secondClient;
    char specialMessage[MAX_LENGTH];
    char buffer[MAX_LENGTH];
    for (int i = 0; i < MAX_LENGTH; i++)
    {
        specialMessage[i] = '0';
    }

    printf("Adres pierwszego gracza: %s\n", inet_ntoa((struct in_addr)firstClient->caddr.sin_addr));
    printf("Adres drugiego gracza: %s\n", inet_ntoa((struct in_addr)secondClient->caddr.sin_addr));

    int endOfGame = 0;
    specialMessage[0] = 'F';
    endOfGame = writeMessage(firstClient->cfd, specialMessage, sizeof(specialMessage));

    if (endOfGame == 1)
    {
        specialMessage[0] = 'L';
        writeMessage(secondClient->cfd, specialMessage, sizeof(specialMessage));
        close(secondClient->cfd);
        close(firstClient->cfd);
        free(firstClient);
        free(secondClient);
        return EXIT_SUCCESS;
    }
    else
    {
        specialMessage[0] = 'D';
        endOfGame = writeMessage(secondClient->cfd, specialMessage, sizeof(specialMessage));

        if (endOfGame == 1)
        {
            specialMessage[0] = 'L';
            writeMessage(firstClient->cfd, specialMessage, sizeof(specialMessage));
            close(secondClient->cfd);
            close(firstClient->cfd);
            free(firstClient);
            free(secondClient);
            return EXIT_SUCCESS;
        }
    }

    int firstUserError;
    int secondUserError;
    int isFirstUser = 1;
    printf("Wiadomosci wyslana do graczy\n");

    printf("Start\n");
    printf("Zaczyna bialy\n");

    while (1)
    {

        if (isFirstUser == 1)
        {

            firstUserError = readMessage(firstClient->cfd, buffer, sizeof(buffer));
            printf("Ruch pierwszego gracza: %s\n", buffer);

            if (buffer[0] == 'K')
            {
                writeMessage(secondClient->cfd, buffer, sizeof(buffer));
                break;
            }
            else if (buffer[0] == 'T')
            {
                isFirstUser = isFirstUser == 0 ? 1 : 0;
                printf("Koniec tury gracza pierwszego\n");
                printf("isFirstUser: %d a powinien wynosic 0\n", isFirstUser);
            }
            else
            {
                printf("Wysylam do drugiego gracza: %s\n", buffer);
                writeMessage(secondClient->cfd, buffer, sizeof(buffer));
            }
        }
        else
        {

            secondUserError = readMessage(secondClient->cfd, buffer, sizeof(buffer));
            printf("Ruch drugiego gracza: %s\n", buffer);
            if (buffer[0] == 'K')
            {
                writeMessage(firstClient->cfd, buffer, sizeof(buffer));
                break;
            }
            else if (buffer[0] == 'T')
            {
                isFirstUser = isFirstUser == 0 ? 1 : 0;
                printf("Koniec tury gracza drugiego\n");
                printf("isFirstUser: %d a powinien wynosic 1\n", isFirstUser);
            }
            else
            {
                printf("Wysylam do pierwszego gracza: %s\n", buffer);
                writeMessage(firstClient->cfd, buffer, sizeof(buffer));
            }
        }
    }

    printf("Gra zakonczona.\n");
    close(secondClient->cfd);
    close(firstClient->cfd);
    free(firstClient);
    free(secondClient);

    return EXIT_SUCCESS;
}
int main(int argc, char **argv)
{
    socklen_t slt;
    pthread_t tid;
    int sfd, on = 1;

    struct sockaddr_in saddr;
    sfd = socket(AF_INET, SOCK_STREAM, 0);
    saddr.sin_family = AF_INET;
    saddr.sin_addr.s_addr = INADDR_ANY;
    saddr.sin_port = htons(1234);
    setsockopt(sfd, SOL_SOCKET, SO_REUSEADDR, (char *)&on, sizeof(on));
    bind(sfd, (struct sockaddr *)&saddr, sizeof(saddr));
    listen(sfd, 11);

    int InLobby = -1;

    while (1)
    {
        struct cln *c = malloc(sizeof(struct cln));
        slt = sizeof(c->caddr);

        c->cfd = accept(sfd, (struct sockaddr *)&c->caddr, &slt);
        InLobby = InLobby + 1;
        lobby[InLobby] = c;

        printf("Nowy gracz w lobby\n");

        if (InLobby == 1)
        {
            struct clns *clients = malloc(sizeof(struct clns));
            clients->firstClient = lobby[0];
            clients->secondClient = lobby[1];
            InLobby = -1;
            pthread_create(&tid, NULL, serverThread, clients);
            pthread_detach(tid);
            printf("Utworzono pokoj rozgrywki.\n");
        }
    }
    close(sfd);
    return EXIT_SUCCESS;
}
