import React, { useState } from 'react'
import { Button, Form, FormControl, InputGroup } from 'react-bootstrap'
import { useHistory } from 'react-router-dom'
import { groupsSlice } from 'store/slices'
import { useAppDispatch } from 'store/store'
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
      <Button
        onClick={() => {
          setShowPrompt(true)
        }}
        variant="primary"
      >
        Add group
      </Button>
      {renderAddPrompt()}
    </>
  )

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
          onChange={event => {
            formik.values.name = event.target.value
            formik.handleChange
          }}
          onBlur={formik.handleBlur}
          isInvalid={
            !!formik.touched.name &&
            (!!formik.errors.name || !!formik.errors.global)
          }
          required
        />
        <Form.Control.Feedback type="invalid">
          {formik.errors.global}
          {formik.errors.name}
        </Form.Control.Feedback>
      </InputGroup>
    </Form>
  )
}

export default AddGroupBtn
