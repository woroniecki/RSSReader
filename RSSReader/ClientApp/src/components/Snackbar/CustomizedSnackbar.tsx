import { Button, Snackbar } from '@material-ui/core'
import { Alert } from '@material-ui/lab'
import React, { useState } from 'react'
import { useSelector } from 'react-redux'
import { useHistory } from 'react-router-dom'
import { snackbarSlice } from 'store/slices'
import { useAppDispatch } from 'store/store'

export interface CustomizedSnackbarProps {}

export const CustomizedSnackbar: React.FC<CustomizedSnackbarProps> = props => {
  const dispatch = useAppDispatch()
  const { push } = useHistory()

  const snackbar = useSelector(snackbarSlice.stateSelector)

  const handleClose = (event?: React.SyntheticEvent, reason?: string) => {
    dispatch(snackbarSlice.actions.setOpen(false))
  }

  return (
    <>
      <Snackbar
        anchorOrigin={{ vertical: 'bottom', horizontal: 'right' }}
        open={snackbar.open}
        autoHideDuration={7000}
        onClose={handleClose}
      >
        <Alert onClose={handleClose} severity={snackbar.color}>
          {snackbar.msg}
        </Alert>
      </Snackbar>
    </>
  )
}

export default CustomizedSnackbar
