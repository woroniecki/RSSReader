import React, { Component } from 'react'
import { Button, Form } from 'react-bootstrap'
import { useFormik } from 'formik'
import axios from 'axios'
import { useHistory } from 'react-router-dom'

import { authSlice } from 'store/slices'
import { useAppDispatch } from 'store/store'

export interface LoginProps {}

export const Login: React.FC<LoginProps> = props => {
  const { push } = useHistory()
  const dispatch = useAppDispatch()
  const formik = useFormik({
    initialValues: {
      usernameoremail: '',
      password: '',
    },
    onSubmit: values => {
      dispatch(
        authSlice.login({
          username: values.usernameoremail,
          password: values.password,
        })
      )
    },
  })

  return (
    <div className="container">
      <Form id="LoginForm" onSubmit={formik.handleSubmit}>
        <Form.Group>
          <Form.Label>Username</Form.Label>
          <Form.Control
            type="text"
            placeholder="Username or email"
            id="usernameoremail"
            name="usernameoremail"
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

export default Login
