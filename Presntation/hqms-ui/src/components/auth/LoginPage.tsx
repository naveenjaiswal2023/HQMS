import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Form, Button, Alert, Card, InputGroup } from 'react-bootstrap';
import api from '../../api/api';

const LoginPage: React.FC = () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [showPassword, setShowPassword] = useState(false);
  const [error, setError] = useState('');
  const navigate = useNavigate();

  useEffect(() => {
    const token = localStorage.getItem('token');
    const storedRole = localStorage.getItem('role');
    if (token && storedRole) {
      redirectToDashboard(storedRole);
    }
  }, [navigate]);

  const redirectToDashboard = (role: string) => {
    if (role === 'Doctor') navigate('/doctor');
    else if (role === 'POD') navigate('/pod-admin');
    else navigate('/patient');
  };

  const handleLogin = async (e: React.FormEvent) => {
  e.preventDefault();
  setError('');
  try {
    const res = await api.post('/auth/api/Auth/login', {
      username: email,           // 👈 rename to match backend
      password: password,
      rememberMe: true
    });
    
    localStorage.setItem('token', res.data.token);
    localStorage.setItem('role', res.data.role);

    const role = res.data.role;
    if (role === 'Doctor') navigate('/doctor');
    else if (role === 'POD') navigate('/pod-admin');
    else navigate('/patient');

  } catch {
    setError('Invalid email or password');
  }
};


  return (
    <div className="d-flex justify-content-center align-items-center vh-100 bg-light">
      <Card className="shadow p-4" style={{ width: '100%', maxWidth: '400px' }}>
        <Card.Body>
          <h3 className="text-center mb-4">HQMS Login</h3>

          {error && <Alert variant="danger">{error}</Alert>}

          <Form onSubmit={handleLogin}>
            <Form.Group className="mb-3">
              <Form.Label>Email</Form.Label>
              <Form.Control
                type="email"
                required
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                placeholder="Enter your email"
              />
            </Form.Group>

            <Form.Group className="mb-3">
              <Form.Label>Password</Form.Label>
              <InputGroup>
                <Form.Control
                  type={showPassword ? 'text' : 'password'}
                  required
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  placeholder="Enter your password"
                />
                <Button
                  variant="outline-secondary"
                  onClick={() => setShowPassword(!showPassword)}
                  tabIndex={-1}
                >
                  <i className={`bi ${showPassword ? 'bi-eye-slash' : 'bi-eye'}`}></i>
                </Button>
              </InputGroup>
            </Form.Group>

            <Button type="submit" className="w-100" variant="primary">
              Login
            </Button>
          </Form>
        </Card.Body>
        <Card.Footer className="text-muted text-center small">
          © {new Date().getFullYear()} HQMS
        </Card.Footer>
      </Card>
    </div>
  );
};

export default LoginPage;
