import React from 'react';
import { Spinner } from 'react-bootstrap';

const Loader: React.FC = () => (
  <div className="d-flex justify-content-center align-items-center" style={{ height: '100vh' }}>
    <Spinner animation="border" variant="primary" />
  </div>
);

export default Loader;