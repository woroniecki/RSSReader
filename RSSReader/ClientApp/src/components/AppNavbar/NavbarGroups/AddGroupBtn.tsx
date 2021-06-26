import React, { useState } from 'react'
import { Button, ListItemText, ListItem, ListItemIcon } from '@material-ui/core'
import { Form, FormControl, InputGroup } from 'react-bootstrap'
import { useHistory } from 'react-router-dom'
import { groupsSlice } from 'store/slices'
import { useAppDispatch } from 'store/store'
import PostAddIcon from '@material-ui/icons/PostAdd'
import { applyValidationErrors } from 'utils/utils'
import { useFormik } from 'formik'
import AddGroupFormPrompt from './AddGroupFormPrompt'

export interface AddGroupBtnProps {}

export const AddGroupBtn: React.FC<AddGroupBtnProps> = props => {
  const dispatch = useAppDispatch()
  const { push } = useHistory()
  const [showPrompt, setShowPrompt] = useState(false)

  const formik = useFormik({
    initialValues: {
      global: '',
      name: '',
    },
    onSubmit: async values => {
      const promise = await dispatch(
        groupsSlice.postAdd({
          name: values.name,
        })
      )

      if (groupsSlice.postAdd.fulfilled.match(promise)) {
      } else {
        applyValidationErrors(formik, promise.error)
      }
    },
  })

  const renderAddPrompt = () => {
    if (showPrompt) {
      return <AddGroupFormPrompt onClose={() => setShowPrompt(false)} />
    }
  }

  return (
    <>
      <ListItem button key="Register" onClick={() => setShowPrompt(true)}>
        <ListItemIcon>
          <PostAddIcon />
        </ListItemIcon>
        <ListItemText primary="New group" />
      </ListItem>
      {renderAddPrompt()}
    </>
  )
}

export default AddGroupBtn
