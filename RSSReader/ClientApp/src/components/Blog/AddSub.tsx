import React, { useState } from 'react'
import { useHistory } from 'react-router-dom'
import { useAppDispatch } from 'store/store'
import { useFormik } from 'formik'
import * as Yup from 'yup'
import { subscriptionsSlice } from 'store/slices'
import { applyValidationErrors } from 'utils/utils'
import { useSelector } from 'react-redux'
import { authSlice } from 'store/slices'
import SpinnerElement from 'components/Spinner/SpinnerElement'
import {
  Button,
  FormHelperText,
  InputAdornment,
  TextField,
} from '@material-ui/core'

export interface AddSubProps {
  activeGroupId: string
}

export const AddSub: React.FC<AddSubProps> = props => {
  const dispatch = useAppDispatch()
  const { push } = useHistory()
  const { userName } = useSelector(authSlice.stateSelector)
  const [isInAction, setIsInAction] = useState(false)

  const formik = useFormik({
    initialValues: {
      global: '',
      url: '',
    },
    validationSchema: Yup.object().shape({
      url: Yup.string().required('Required'),
    }),
    onSubmit: async values => {
      if (isInAction) return
      setIsInAction(true)

      let groupIdToSend = parseInt(props.activeGroupId)
      groupIdToSend = groupIdToSend == -1 ? null : groupIdToSend

      const promise = await dispatch(
        subscriptionsSlice.postAddSubscription({
          blogUrl: values.url,
          GroupId: groupIdToSend,
        })
      )

      if (subscriptionsSlice.postAddSubscription.fulfilled.match(promise)) {
      } else {
        applyValidationErrors(formik, promise.error)
      }
      setIsInAction(false)
    },
  })

  function getSubmitBtnBody() {
    if (!isInAction) return '+'
    return <SpinnerElement variant="primary" size={12} />
  }

  return (
    <form noValidate autoComplete="off" onSubmit={formik.handleSubmit}>
      <TextField
        label="Subscribe"
        fullWidth
        type="text"
        placeholder="https://exampleblog.com/feed/"
        id="url"
        name="url"
        onChange={formik.handleChange}
        onBlur={formik.handleBlur}
        value={formik.values.url}
        error={
          !!formik.touched.url &&
          (!!formik.errors.url || !!formik.errors.global)
        }
        InputProps={{
          startAdornment: (
            <InputAdornment position="start">
              <Button type="submit">{getSubmitBtnBody()}</Button>
            </InputAdornment>
          ),
        }}
      />
      <FormHelperText error id="component-error-text">
        {formik.errors.global}
        {formik.errors.url}
      </FormHelperText>
    </form>
  )
}

export default AddSub
