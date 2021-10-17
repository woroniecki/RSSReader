import React, { useState } from 'react'
import { useHistory } from 'react-router-dom'
import { useAppDispatch } from 'store/store'
import { useFormik } from 'formik'
import * as Yup from 'yup'
import { blogsSlice } from 'store/slices'
import { applyValidationErrors } from 'utils/utils'
import { useSelector } from 'react-redux'
import { authSlice } from 'store/slices'
import { getSearchBlogs } from '../../api/blogApi'
import SpinnerElement from 'components/Spinner/SpinnerElement'
import {
  Box,
  Button,
  FormHelperText,
  InputAdornment,
  TextField,
} from '@material-ui/core'
import { Autocomplete } from '@material-ui/lab'
import { BlogSearchResponse } from 'api/api.types'
import BlogAvatar from './BlogAvatar'
import { makeStyles, Theme, createStyles } from '@material-ui/core/styles'

export interface AddSubProps {
  activeGroupId: string
}

export const AddSub: React.FC<AddSubProps> = props => {
  const dispatch = useAppDispatch()
  const [isInAction, setIsInAction] = useState(false)
  const [hintsList, setHintsList] = useState<BlogSearchResponse[]>([])
  const classes = useStyles()

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
        blogsSlice.postAddSubscription({
          blogUrl: values.url,
          GroupId: groupIdToSend,
        })
      )

      if (blogsSlice.postAddSubscription.fulfilled.match(promise)) {
      } else {
        applyValidationErrors(formik, promise.error)
      }
      setIsInAction(false)
    },
  })

  function getSubmitBtnBody() {
    if (!isInAction) return '+'
    return <SpinnerElement size={12} />
  }

  function setUrlFieldValue(data: any) {
    try {
      const value = data as BlogSearchResponse
      formik.setFieldValue('url', value.url)
    } catch (err) {
      console.error(err)
    }
  }

  async function onChangeGetHintsList(event: React.ChangeEvent<any>) {
    try {
      const value = event.target.value as string
      if (value.length < 1) {
        setHintsList([])
        return
      }
      const list = (await getSearchBlogs(value)) as BlogSearchResponse[]
      setHintsList(list)
    } catch (err) {
      console.error(err)
    }
  }

  return (
    <form noValidate autoComplete="off" onSubmit={formik.handleSubmit}>
      <Autocomplete
        freeSolo
        id="autocomplete-url"
        options={hintsList}
        onChange={(_, value) => {
          setUrlFieldValue(value)
        }}
        getOptionLabel={option => option.url}
        renderOption={(props, option) => (
          <Box component="li" className={classes.hintLine} {...props}>
            <BlogAvatar
              title={props.name}
              imageUrl={props.imageUrl}
              size="small"
            />
            <div>{props.name}</div>
          </Box>
        )}
        renderInput={params => {
          return (
            <TextField
              {...params}
              label="Subscribe"
              fullWidth
              placeholder="https://exampleblog.com/feed/"
              id="url"
              name="url"
              onChange={event => {
                formik.handleChange(event)
                onChangeGetHintsList(event)
              }}
              onBlur={formik.handleBlur}
              value={formik.values.url}
              error={
                !!formik.touched.url &&
                (!!formik.errors.url || !!formik.errors.global)
              }
              InputProps={{
                ...params.InputProps,
                startAdornment: (
                  <>
                    <InputAdornment position="start">
                      <Button type="submit">{getSubmitBtnBody()}</Button>
                    </InputAdornment>
                    {params.InputProps.startAdornment}
                  </>
                ),
              }}
            />
          )
        }}
      />
      <FormHelperText error id="component-error-text">
        {formik.errors.global}
        {formik.errors.url}
      </FormHelperText>
    </form>
  )
}

const useStyles = makeStyles((theme: Theme) =>
  createStyles({
    hintLine: {
      display: 'flex',
      '& > *': {
        margin: theme.spacing(1),
      },
    },
  })
)

export default AddSub
