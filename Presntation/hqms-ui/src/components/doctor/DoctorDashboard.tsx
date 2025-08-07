import React, { useEffect, useState } from 'react';
import { Card, Table, Button, Badge, Row, Col } from 'react-bootstrap';
import { connectSignalR } from '../../utils/signalr';
import api from '../../api/api';
import type { Patient, QueueStats } from '../../types/models';

const DoctorDashboard: React.FC = () => {
  const [queue, setQueue] = useState<Patient[]>([]);
  const [stats, setStats] = useState<QueueStats>({ total: 0, waiting: 0, completed: 0 });

  useEffect(() => {
    fetchQueue();

    const connection = connectSignalR<{ queue: Patient[]; stats: QueueStats }>(
      '/queueHub',
      (data) => {
        setQueue(data.queue || []);
        setStats(data.stats || { total: 0, waiting: 0, completed: 0 });
      }
    );

    return () => {
      void connection.stop(); // graceful shutdown
    };
  }, []);

  const fetchQueue = async () => {
    try {
      const res = await api.get('/doctor/queue');
      setQueue(res.data.queue || []);
      setStats(res.data.stats || { total: 0, waiting: 0, completed: 0 });
    } catch (error) {
      console.error('Failed to fetch doctor queue:', error);
    }
  };

  const callNextPatient = async () => {
    try {
      await api.post('/doctor/call-next');
      // Fallback update if SignalR is delayed
      fetchQueue();
    } catch (error) {
      console.error('Failed to call next patient:', error);
    }
  };

  return (
    <div className="container mt-4">
      <Row>
        <Col md={4}>
          <Card>
            <Card.Body>
              <h5>Queue Summary</h5>
              <div>Total Patients: <Badge bg="primary">{stats.total}</Badge></div>
              <div>Waiting: <Badge bg="danger">{stats.waiting}</Badge></div>
              <div>Completed: <Badge bg="success">{stats.completed}</Badge></div>
              <Button className="mt-3 w-100" onClick={callNextPatient}>
                Call Next Patient
              </Button>
            </Card.Body>
          </Card>
        </Col>
        <Col md={8}>
          <Card>
            <Card.Body>
              <h5>Live Queue</h5>
              <Table striped bordered hover responsive>
                <thead>
                  <tr>
                    <th>Token</th>
                    <th>Name</th>
                    <th>Status</th>
                    <th>Appointment Time</th>
                  </tr>
                </thead>
                <tbody>
                  {queue.length > 0 ? (
                    queue.map((patient) => (
                      <tr key={patient.token}>
                        <td>{patient.token}</td>
                        <td>{patient.name}</td>
                        <td>
                          <Badge
                            bg={
                              patient.status === 'Completed'
                                ? 'success'
                                : patient.status === 'Called'
                                ? 'warning'
                                : 'danger'
                            }
                          >
                            {patient.status}
                          </Badge>
                        </td>
                        <td>{patient.appointmentTime}</td>
                      </tr>
                    ))
                  ) : (
                    <tr>
                      <td colSpan={4} className="text-center text-muted">
                        No patients in the queue.
                      </td>
                    </tr>
                  )}
                </tbody>
              </Table>
            </Card.Body>
          </Card>
        </Col>
      </Row>
    </div>
  );
};

export default DoctorDashboard;
