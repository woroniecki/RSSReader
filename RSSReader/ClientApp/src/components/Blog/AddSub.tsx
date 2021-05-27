import React from 'react'
import { InputGroup, Button, FormControl, Form } from 'react-bootstrap'
import { useHistory } from 'react-router-dom'
import { useAppDispatch } from 'store/store'
import { useFormik } from 'formik'
import * as Yup from 'yup'
import { subscriptionsSlice } from 'store/slices'
import { applyValidationErrors } from 'utils/utils'
import { useSelector } from 'react-redux'
import { authSlice } from 'store/slices'

export interface AddSubProps {
  activeGroupId: string
}

export const AddSub: React.FC<AddSubProps> = props => {
  const dispatch = useAppDispatch()
  const { push } = useHistory()
  const { userName } = useSelector(authSlice.stateSelector)

  const formik = useFormik({
    initialValues: {
      global: '',
      url: '',
    },
    //validationSchema: Yup.object().shape({
    //  url: Yup.string().required('Required'),
    //}),
    onSubmit: async values => {
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
    },
  })

  return (
    <Form id="AddSub" onSubmit={formik.handleSubmit}>
      <InputGroup className="mb-3">
        <InputGroup.Prepend>
          <Button
            //disabled={isLoading}
            //onClick={!isLoading ? handleClick : null}
            variant="outline-primary"
            type="submit"
          >
            +
          </Button>
        </InputGroup.Prepend>
        <FormControl
          type="text"
          aria-describedby="basic-addon1"
          placeholder="https://exampleblog.com/feed/"
          id="url"
          name="url"
          onChange={formik.handleChange}
          onBlur={formik.handleBlur}
          value={formik.values.url}
          isInvalid={
            !!formik.touched.url &&
            (!!formik.errors.url || !!formik.errors.global)
          }
          required
        />
        <Form.Control.Feedback type="invalid">
          {formik.errors.global}
          {formik.errors.url}
        </Form.Control.Feedback>
      </InputGroup>
    </Form>
  )
}

export default AddSub
