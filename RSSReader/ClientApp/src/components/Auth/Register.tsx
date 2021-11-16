import {
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  FormHelperText,
  TextField,
} from '@material-ui/core'
import Alert from '@material-ui/lab/Alert'
import { useFormik } from 'formik'
import React from 'react'
import { useHistory } from 'react-router-dom'
import { layoutSlice, snackbarSlice } from 'store/slices'
import { useAppDispatch } from 'store/store'
import { applyValidationErrors } from 'utils/utils'
import * as Yup from 'yup'
import { register } from '../../api/authApi'

export interface RegisterProps {}

export const Register: React.FC<RegisterProps> = () => {
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
        if (err && err.data) applyValidationErrors(formik, err.data)
        dispatch(
          snackbarSlice.actions.setSnackbar({
            open: true,
            color: 'error',
            msg: 'Register failed',
          })
        )
      }
      dispatch(layoutSlice.actions.setLoader(layoutSlice.type.none))
    },
  })

  return (
    <Dialog
      open={true}
      onClose={() => push('/')}
      aria-labelledby="form-dialog-title"
    >
      <form noValidate autoComplete="off" onSubmit={formik.handleSubmit}>
        <DialogContent>
          <TextField
            label="Username"
            fullWidth
            type="text"
            placeholder="Username"
            id="username"
            name="username"
            onChange={formik.handleChange}
            onBlur={formik.handleBlur}
            value={formik.values.username}
            error={!!formik.touched.username && !!formik.errors.username}
            required
          />
          {formik.touched.username && formik.errors.username ? (
            <FormHelperText error id="component-error-text">
              {formik.errors.username}
            </FormHelperText>
          ) : null}

          <TextField
            label="Email"
            fullWidth
            type="text"
            placeholder="email"
            id="email"
            name="email"
            onChange={formik.handleChange}
            onBlur={formik.handleBlur}
            value={formik.values.email}
            error={!!formik.touched.email && !!formik.errors.email}
            required
          />
          {formik.touched.email && formik.errors.email ? (
            <FormHelperText error id="component-error-text">
              {formik.errors.email}
            </FormHelperText>
          ) : null}

          <TextField
            label="Password"
            fullWidth
            type="password"
            placeholder="Password"
            id="password"
            name="password"
            onChange={formik.handleChange}
            onBlur={formik.handleBlur}
            value={formik.values.password}
            error={!!formik.touched.password && !!formik.errors.password}
            required
          />
          {formik.touched.password && formik.errors.password ? (
            <FormHelperText error id="component-error-text">
              {formik.errors.password}
            </FormHelperText>
          ) : null}

          {formik.errors.global != null && (
            <Alert severity="error">{formik.errors.global}</Alert>
          )}
        </DialogContent>
        <DialogActions>
          <Button type="submit">Login</Button>
          <Button onClick={() => push('/')}>Close</Button>
        </DialogActions>
      </form>
    </Dialog>
  )
}

export default Register
