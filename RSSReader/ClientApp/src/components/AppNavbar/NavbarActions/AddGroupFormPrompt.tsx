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
import { useFormik } from 'formik'
import React from 'react'
import { groupsSlice } from 'store/slices'
import { useAppDispatch } from 'store/store'
import { applyValidationErrors } from 'utils/utils'
import * as Yup from 'yup'

export interface AddGroupFormPromptProps {
  onClose: (event: React.MouseEvent<HTMLButtonElement>) => void
}

export const AddGroupFormPrompt: React.FC<AddGroupFormPromptProps> = props => {
  const dispatch = useAppDispatch()

  const formik = useFormik({
    initialValues: {
      global: '',
      name: '',
    },
    validationSchema: Yup.object().shape({
      name: Yup.string().required('Required'),
    }),
    onSubmit: async values => {
      const promise = await dispatch(
        groupsSlice.postAdd({
          name: values.name,
        })
      )

      if (groupsSlice.postAdd.fulfilled.match(promise)) {
        props.onClose(this)
      } else {
        applyValidationErrors(formik, promise.error)
      }
    },
  })

  return (
    <Dialog
      open={true}
      onClose={props.onClose}
      aria-labelledby="form-dialog-title"
    >
      <DialogTitle id="form-dialog-title">Add group</DialogTitle>
      <form noValidate autoComplete="off" onSubmit={formik.handleSubmit}>
        <DialogContent>
          <DialogContentText>Provide new group data</DialogContentText>
          <TextField
            autoFocus
            id="name"
            label="Group name"
            fullWidth
            onChange={event => {
              formik.values.name = event.target.value
              formik.handleChange
            }}
            onBlur={formik.handleBlur}
            error={
              !!formik.touched.name &&
              (!!formik.errors.name || !!formik.errors.global)
            }
            required
          />
          <FormHelperText error id="component-error-text">
            {formik.errors.global}
            {formik.errors.name}
          </FormHelperText>
        </DialogContent>
        <DialogActions>
          <Button type="submit">Add</Button>
          <Button onClick={props.onClose}>Close</Button>
        </DialogActions>
      </form>
    </Dialog>
  )
}

export default AddGroupFormPrompt
