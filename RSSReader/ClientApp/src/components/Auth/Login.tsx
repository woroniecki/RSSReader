import React, { Component } from 'react'
import { Button, Form } from 'react-bootstrap'
import Alert from 'react-bootstrap/Alert'
import { useFormik } from 'formik'
import { useHistory } from 'react-router-dom'
import * as Yup from 'yup'

import { authSlice } from 'store/slices'
import { useAppDispatch } from 'store/store'
import { layoutSlice } from 'store/slices'
import { applyValidationErrors } from 'utils/utils'

export interface LoginProps {}

export const Login: React.FC<LoginProps> = props => {
  const { push } = useHistory()
  const dispatch = useAppDispatch()

  const formik = useFormik({
    initialValues: {
      global: '',
      username: '',
      password: '',
    },
    validationSchema: Yup.object().shape({
      username: Yup.string().required('Required'),
      password: Yup.string().required('Required'),
    }),
    onSubmit: async values => {
      dispatch(layoutSlice.actions.setLoader(true))
      const promise = await dispatch(
        authSlice.login({
          username: values.username,
          password: values.password,
        })
      )
      dispatch(layoutSlice.actions.setLoader(false))

      if (authSlice.login.fulfilled.match(promise)) {
        push('/')
      } else {
        applyValidationErrors(formik, promise.payload)
      }
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
          Login
        </Button>
      </Form>
    </div>
  )
}

export default Login