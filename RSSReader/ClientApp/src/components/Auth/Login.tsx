import {
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
  FormHelperText,
  TextField,
} from '@material-ui/core'
import React, { useState } from 'react'
import { Alert } from '@material-ui/lab'
import { useFormik } from 'formik'
import { useHistory } from 'react-router-dom'
import { authSlice, layoutSlice, snackbarSlice } from 'store/slices'
import { useAppDispatch } from 'store/store'
import { applyValidationErrors } from 'utils/utils'
import SpinnerElement from 'components/Spinner/SpinnerElement'
import * as Yup from 'yup'

export interface LoginProps {}

export const Login: React.FC<LoginProps> = () => {
  const { push } = useHistory()
  const dispatch = useAppDispatch()
  const [isInAction, setIsInAction] = useState(false)

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
      if (isInAction) return
      setIsInAction(true)

      const promise = await dispatch(
        authSlice.login({
          username: values.username,
          password: values.password,
        })
      )

      if (authSlice.login.fulfilled.match(promise)) {
        push('/')
      } else {
        applyValidationErrors(formik, promise.payload)
        dispatch(
          snackbarSlice.actions.setSnackbar({
            open: true,
            color: 'error',
            msg: 'Logging in failed',
          })
        )
      }

      setIsInAction(false)
    },
  })

  function renderSubmitButton() {
    if (!isInAction) return 'Login'
    return <SpinnerElement size={18} />
  }

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
            placeholder="Username or email"
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
          <Button type="submit">{renderSubmitButton()}</Button>
          <Button onClick={() => push('/')}>Close</Button>
        </DialogActions>
      </form>
    </Dialog>
  )
}

export default Login
