import React from 'react'
import { InputGroup, Button, FormControl, Form } from 'react-bootstrap'
import { useHistory } from 'react-router-dom'
import { useAppDispatch } from 'store/store'
import { useFormik } from 'formik'
import * as Yup from 'yup'
import { subscriptionsSlice } from 'store/slices'

export interface AddSubProps {}

export const AddSub: React.FC<AddSubProps> = props => {
  const dispatch = useAppDispatch()
  const { push } = useHistory()

  const formik = useFormik({
    initialValues: {
      global: '',
      url: '',
    },
    validationSchema: Yup.object().shape({
      url: Yup.string().required('Required'),
    }),
    onSubmit: async values => {
      console.log(values)

      const promise = await dispatch(
        subscriptionsSlice.postAddSubscription({
          blogUrl: values.url,
        })
      )

      if (subscriptionsSlice.postAddSubscription.fulfilled.match(promise)) {
      } else {
        console.log(promise.payload)
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
          placeholder="https://exampleblog.com"
          id="url"
          name="url"
          onChange={formik.handleChange}
          onBlur={formik.handleBlur}
          value={formik.values.url}
          isInvalid={!!formik.touched.url && !!formik.errors.url}
          required
        />
      </InputGroup>
    </Form>
  )
}

export default AddSub
