import React, { useState, useEffect } from 'react';
import { Card, Table, Button, Form, Row, Col, Badge, Modal } from 'react-bootstrap';
import api from '../../api/api';
import type { Department, Doctor, Patient } from '../../types/models';

const PodAdminDashboard: React.FC = () => {
  const [departments, setDepartments] = useState<Department[]>([]);
  const [doctors, setDoctors] = useState<Doctor[]>([]);
  const [selectedDept, setSelectedDept] = useState('');
  const [selectedDoctor, setSelectedDoctor] = useState('');
  const [queues, setQueues] = useState<Patient[]>([]);
  const [search, setSearch] = useState('');
  const [showModal, setShowModal] = useState(false);
  const [selectedPatient, setSelectedPatient] = useState<Patient | null>(null);
  const [reassignDoctor, setReassignDoctor] = useState(''); // used only in modal

  useEffect(() => {
    const fetchData = async () => {
      const [deptRes, docRes] = await Promise.all([
        api.get('/departments'),
        api.get('/doctors')
      ]);
      setDepartments(deptRes.data);
      setDoctors(docRes.data);
    };
    fetchData();
    fetchQueues();
  }, []);

  const fetchQueues = async () => {
    const res = await api.get('/pod/queues');
    setQueues(res.data);
  };

  const assignPatient = async (patientId: string) => {
    if (!reassignDoctor) return;
    await api.post('/pod/assign', { patientId, doctorId: reassignDoctor });
    fetchQueues();
    setShowModal(false);
    setReassignDoctor('');
  };

  const cancelPatient = async (patientId: string) => {
    await api.post('/pod/cancel', { patientId });
    fetchQueues();
  };

  const filteredQueues = queues.filter(q =>
    q.name.toLowerCase().includes(search.toLowerCase()) ||
    q.patientId.includes(search) ||
    q.status.toLowerCase().includes(search.toLowerCase())
  );

  const handleOpenModal = (patient: Patient) => {
    setSelectedPatient(patient);
    setReassignDoctor(''); // reset modal state
    setShowModal(true);
  };

  return (
    <div className="container mt-4">
      <Row className="mb-3">
        <Col md={4}>
          <Form.Select value={selectedDept} onChange={e => setSelectedDept(e.target.value)}>
            <option value="">Select Department</option>
            {departments.map(d => (
              <option key={d.id} value={d.id}>{d.name}</option>
            ))}
          </Form.Select>
        </Col>
        <Col md={4}>
          <Form.Select value={selectedDoctor} onChange={e => setSelectedDoctor(e.target.value)} disabled={!selectedDept}>
            <option value="">Select Doctor</option>
            {doctors
              .filter(doc => doc.departmentId === selectedDept)
              .map(doc => (
                <option key={doc.id} value={doc.id}>{doc.name}</option>
              ))}
          </Form.Select>
        </Col>
        <Col md={4}>
          <Form.Control
            placeholder="Search by name, ID, or status"
            value={search}
            onChange={e => setSearch(e.target.value)}
          />
        </Col>
      </Row>

      <Card>
        <Card.Body>
          <h5>Active Queues</h5>
          <Table striped bordered hover responsive>
            <thead>
              <tr>
                <th>Token</th>
                <th>Patient Name</th>
                <th>Status</th>
                <th>Doctor</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {filteredQueues.length === 0 ? (
                <tr>
                  <td colSpan={5} className="text-center text-muted">No queue entries found.</td>
                </tr>
              ) : (
                filteredQueues.map(q => (
                  <tr key={q.token}>
                    <td>{q.token}</td>
                    <td>{q.name}</td>
                    <td>
                      <Badge
                        bg={
                          q.status === 'Completed'
                            ? 'success'
                            : q.status === 'Called'
                            ? 'warning'
                            : 'danger'
                        }
                      >
                        {q.status}
                      </Badge>
                    </td>
                    <td>{q.doctorName || '-'}</td>
                    <td>
                      <Button
                        size="sm"
                        variant="primary"
                        onClick={() => handleOpenModal(q)}
                      >
                        Reassign
                      </Button>{' '}
                      <Button
                        size="sm"
                        variant="danger"
                        onClick={() => cancelPatient(q.patientId)}
                      >
                        Cancel
                      </Button>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </Table>
        </Card.Body>
      </Card>

      {/* Modal for reassignment */}
      <Modal show={showModal} onHide={() => setShowModal(false)} centered>
        <Modal.Header closeButton>
          <Modal.Title>Reassign Patient</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <Form.Group className="mb-3">
            <Form.Label>Select Doctor</Form.Label>
            <Form.Select value={reassignDoctor} onChange={e => setReassignDoctor(e.target.value)}>
              <option value="">Select Doctor</option>
              {doctors.map(doc => (
                <option key={doc.id} value={doc.id}>{doc.name}</option>
              ))}
            </Form.Select>
          </Form.Group>
        </Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={() => setShowModal(false)}>
            Close
          </Button>
          <Button
            variant="primary"
            disabled={!reassignDoctor}
            onClick={() => selectedPatient && assignPatient(selectedPatient.patientId)}
          >
            Assign
          </Button>
        </Modal.Footer>
      </Modal>
    </div>
  );
};

export default PodAdminDashboard;
