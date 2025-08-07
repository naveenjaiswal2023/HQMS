import React from 'react';
import { Navbar, Container } from 'react-bootstrap';

const AppNavbar: React.FC = () => (
  <Navbar bg="dark" variant="dark">
    <Container>
      <Navbar.Brand>HQMS</Navbar.Brand>
    </Container>
  </Navbar>
);

export default AppNavbar;