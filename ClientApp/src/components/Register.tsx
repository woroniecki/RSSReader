import React, { Component } from 'react'
import { Button, Form } from 'react-bootstrap'
import { useFormik } from 'formik'
import axios from 'axios'
import { useHistory } from 'react-router-dom'

import { register } from '../api/authApi'

export interface RegisterProps {}

export const Register: React.FC<RegisterProps> = props => {
  const { push } = useHistory()

  const formik = useFormik({
    initialValues: {
      username: '',
      email: '',
      password: '',
    },
    onSubmit: values => {
      register(values)
    },
  })

  return (
    <div className="container">
      <Form id="registerForm" onSubmit={formik.handleSubmit}>
        <Form.Group>
          <Form.Label>Username</Form.Label>
          <Form.Control
            type="text"
            placeholder="Username"
            id="username"
            name="username"
            onChange={formik.handleChange}
          />
        </Form.Group>

        <Form.Group>
          <Form.Label>Email address</Form.Label>
          <Form.Control
            type="email"
            placeholder="Enter email"
            id="email"
            name="email"
            onChange={formik.handleChange}
          />
        </Form.Group>

        <Form.Group>
          <Form.Label>Password</Form.Label>
          <Form.Control
            type="password"
            placeholder="Password"
            id="password"
            name="password"
            onChange={formik.handleChange}
          />
        </Form.Group>

        <Button variant="primary" type="submit">
          Submit
        </Button>
      </Form>
    </div>
  )
}

export default Register
