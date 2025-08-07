import * as signalR from '@microsoft/signalr';

export function connectSignalR<T>(hubUrl: string, onUpdate: (data: T) => void) {
  const connection = new signalR.HubConnectionBuilder()
    .withUrl(hubUrl, { accessTokenFactory: () => localStorage.getItem('token') || '' })
    .withAutomaticReconnect()
    .build();

  connection.on('QueueUpdated', onUpdate);

  connection.start().catch(console.error);

  return connection;
}