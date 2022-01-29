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
import { useHistory, useParams } from 'react-router-dom'
import { authSlice, layoutSlice, snackbarSlice } from 'store/slices'
import { useAppDispatch } from 'store/store'
import { applyValidationErrors } from 'utils/utils'
import SpinnerElement from 'components/Spinner/SpinnerElement'
import * as Yup from 'yup'

export interface AutoLoginProps {}

export const AutoLogin: React.FC<AutoLoginProps> = () => {
  const { push } = useHistory()
  const dispatch = useAppDispatch()
  const { username, password } = useParams<{ username: string, password: string }>()
  const [isInAction, setIsInAction] = useState(false)
  
  const loginAction = async () => {
    if (isInAction) return
      setIsInAction(true)

      const promise = await dispatch(
        authSlice.login({
          username: username,
          password: password,
        })
      )

      if (authSlice.login.fulfilled.match(promise)) {
      } else {
        dispatch(
          snackbarSlice.actions.setSnackbar({
            open: true,
            color: 'error',
            msg: 'Logging in failed',
          })
        )
      }

      push('/')

      setIsInAction(false)
  }

  React.useEffect(() => {
    loginAction()
  }, [])

  return (<></>)
}

export default AutoLogin
