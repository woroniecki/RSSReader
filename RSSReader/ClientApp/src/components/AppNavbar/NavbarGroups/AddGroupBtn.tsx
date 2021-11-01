import React, { useState } from 'react'
import { ListItemText, ListItem, ListItemIcon } from '@material-ui/core'
import { groupsSlice } from 'store/slices'
import { useAppDispatch } from 'store/store'
import PostAddIcon from '@material-ui/icons/PostAdd'
import { applyValidationErrors } from 'utils/utils'
import { useFormik } from 'formik'
import AddGroupFormPrompt from './AddGroupFormPrompt'
import AppListItemIcon from '../AppListItemIcon'
import AppListItemText from '../AppListItemText'

export interface AddGroupBtnProps {}

export const AddGroupBtn: React.FC<AddGroupBtnProps> = () => {
  const dispatch = useAppDispatch()
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
        <AppListItemIcon>
          <PostAddIcon />
        </AppListItemIcon>
        <AppListItemText fontSize={16} variant="h2" text="Add group" />
      </ListItem>
      {renderAddPrompt()}
    </>
  )
}

export default AddGroupBtn
