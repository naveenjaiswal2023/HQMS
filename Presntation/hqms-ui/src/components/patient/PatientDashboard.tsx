import React, { useEffect, useState } from 'react';
import { Card, Badge, Alert } from 'react-bootstrap';
import { connectSignalR } from '../../utils/signalr';
import api from '../../api/api';
import type { QueueInfo } from '../../types/models';

const PatientDashboard: React.FC = () => {
  const [queueInfo, setQueueInfo] = useState<QueueInfo | null>(null);
  const [notification, setNotification] = useState('');

  useEffect(() => {
    fetchQueueInfo();

    const connection = connectSignalR<QueueInfo>('/queueHub', (data) => {
      setQueueInfo(data);
      if (data.status === 'Called') {
        setNotification('🎉 Your token has been called!');
      }
    });

    return () => {
      void connection.stop(); // Async-safe cleanup
    };
  }, []);

  const fetchQueueInfo = async () => {
    try {
      const res = await api.get('/patient/queue');
      setQueueInfo(res.data);
    } catch (error) {
      console.error('Failed to fetch queue info:', error);
    }
  };

  const getStatusColor = (status: string | undefined) => {
    if (status === 'Completed') return 'success';
    if (status === 'Called') return 'warning';
    return 'danger';
  };

  return (
    <div className="container mt-4">
      <Card>
        <Card.Body>
          <h5>Queue Status</h5>
          {queueInfo ? (
            <>
              <div>
                Token: <Badge bg="info">{queueInfo.token}</Badge>
              </div>
              <div>Doctor: {queueInfo.doctorName || 'N/A'}</div>
              <div>Department: {queueInfo.departmentName || 'N/A'}</div>
              <div>
                Estimated Wait:{' '}
                <Badge bg="warning">
                  {queueInfo.estimatedWaitTime ?? '--'} min
                </Badge>
              </div>
              <div>
                Status:{' '}
                <Badge bg={getStatusColor(queueInfo.status)}>
                  {queueInfo.status}
                </Badge>
              </div>
            </>
          ) : (
            <p className="text-muted">No queue information available.</p>
          )}
          {notification && (
            <Alert variant="success" className="mt-3">
              {notification}
            </Alert>
          )}
        </Card.Body>
      </Card>
    </div>
  );
};

export default PatientDashboard;
