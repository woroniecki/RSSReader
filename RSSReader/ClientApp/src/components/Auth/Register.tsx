import React, { Component } from 'react'
import { Button, Form } from 'react-bootstrap'
import Alert from 'react-bootstrap/Alert'
import { useFormik } from 'formik'
import axios from 'axios'
import { useHistory } from 'react-router-dom'
import { useAppDispatch } from 'store/store'
import { layoutSlice } from 'store/slices'
import * as Yup from 'yup'
import { register } from '../../api/authApi'
import { applyValidationErrors } from 'utils/utils'

export interface RegisterProps {}

export const Register: React.FC<RegisterProps> = props => {
  const { push } = useHistory()
  const dispatch = useAppDispatch()

  const formik = useFormik({
    initialValues: {
      global: '',
      username: '',
      email: '',
      password: '',
    },
    validationSchema: Yup.object().shape({
      username: Yup.string()
        .min(3, 'Min 3 characters')
        .max(20, 'Max 20 characters')
        .required('Required'),
      email: Yup.string().email('Invalid email address').required('Required'),
      password: Yup.string()
        .min(3, 'Min 3 characters')
        .max(20, 'Max 20 characters')
        .required('Required'),
    }),
    onSubmit: async values => {
      dispatch(layoutSlice.actions.setLoader(layoutSlice.type.partial))
      try {
        await register(values)
        push('/login')
      } catch (err) {
        applyValidationErrors(formik, err.data)
      }
      dispatch(layoutSlice.actions.setLoader(layoutSlice.type.none))
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
            onBlur={formik.handleBlur}
            value={formik.values.username}
            isInvalid={!!formik.touched.username && !!formik.errors.username}
            required
          />
          {formik.touched.username && formik.errors.username ? (
            <Form.Control.Feedback type="invalid">
              {formik.errors.username}
            </Form.Control.Feedback>
          ) : null}
        </Form.Group>

        <Form.Group>
          <Form.Label>Email address</Form.Label>
          <Form.Control
            type="email"
            placeholder="Enter email"
            id="email"
            name="email"
            onChange={formik.handleChange}
            onBlur={formik.handleBlur}
            value={formik.values.email}
            isInvalid={!!formik.touched.email && !!formik.errors.email}
            required
          />
          {formik.touched.email && formik.errors.email ? (
            <Form.Control.Feedback type="invalid">
              {formik.errors.email}
            </Form.Control.Feedback>
          ) : null}
        </Form.Group>

        <Form.Group>
          <Form.Label>Password</Form.Label>
          <Form.Control
            type="password"
            placeholder="Password"
            id="password"
            name="password"
            onChange={formik.handleChange}
            onBlur={formik.handleBlur}
            value={formik.values.password}
            isInvalid={!!formik.touched.password && !!formik.errors.password}
            required
          />
          {formik.touched.password && formik.errors.password ? (
            <Form.Control.Feedback type="invalid">
              {formik.errors.password}
            </Form.Control.Feedback>
          ) : null}
        </Form.Group>

        <Alert show={formik.errors.global != null} variant="danger">
          {formik.errors.global}
        </Alert>

        <Button variant="primary" type="submit">
          Register
        </Button>
      </Form>
    </div>
  )
}

export default Register
