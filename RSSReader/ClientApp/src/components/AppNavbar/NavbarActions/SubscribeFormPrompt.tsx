import React, { useState } from 'react'
import { useAppDispatch } from 'store/store'
import { useFormik } from 'formik'
import { groupsSlice } from 'store/slices'
import { blogsSlice } from 'store/slices'
import { applyValidationErrors } from 'utils/utils'
import * as Yup from 'yup'
import {
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
  FormHelperText,
  TextField,
  Box,
  InputAdornment,
  Select,
  MenuItem,
  FormControl,
} from '@material-ui/core'
import { useSelector } from 'react-redux'
import InputLabel from '@material-ui/core/InputLabel'
import { Autocomplete } from '@material-ui/lab'
import { BlogSearchResponse } from 'api/api.types'
import { makeStyles, Theme, createStyles } from '@material-ui/core/styles'
import { snackbarSlice } from 'store/slices'
import { getSearchBlogs } from 'api/blogApi'
import BlogAvatar from 'components/Blog/BlogAvatar'
import SpinnerElement from 'components/Spinner/SpinnerElement'
import { useParams } from 'react-router-dom'

export interface SubscribeFormPromptProps {
  onClose: (event: React.MouseEvent<HTMLButtonElement>) => void
}

export const SubscribeFormPrompt: React.FC<SubscribeFormPromptProps> = props => {
  const dispatch = useAppDispatch()
  const [isInAction, setIsInAction] = useState(false)
  const [hintsList, setHintsList] = useState<BlogSearchResponse[]>([])
  const groupsList = useSelector(groupsSlice.selectAll)
  const classes = useStyles()
  const { groupId } = useParams<{ groupId: string }>()

  function getDefaultGroup(): number {
    const value = parseInt(groupId)
    if (isNaN(value)) return -1
    return value
  }

  const formik = useFormik({
    initialValues: {
      global: '',
      url: '',
      group: getDefaultGroup(),
    },
    validationSchema: Yup.object().shape({
      url: Yup.string().required('Required'),
    }),
    onSubmit: async values => {
      if (isInAction) return
      setIsInAction(true)

      const promise = await dispatch(
        blogsSlice.postAddSubscription({
          blogUrl: values.url,
          GroupId: values.group,
        })
      )

      if (blogsSlice.postAddSubscription.fulfilled.match(promise)) {
        props.onClose(this)
        dispatch(
          snackbarSlice.actions.setSnackbar({
            open: true,
            color: 'success',
            msg: 'Subscription added',
          })
        )
      } else {
        applyValidationErrors(formik, promise.error)
        dispatch(
          snackbarSlice.actions.setSnackbar({
            open: true,
            color: 'error',
            msg: 'Adding subscription failed',
          })
        )
      }
      setIsInAction(false)
    },
  })

  function renderSubmitButton() {
    if (!isInAction) return 'Submit'
    return <SpinnerElement size={18} />
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

  const renderGroupsList = () => {
    return groupsList.map(el => (
      <MenuItem key={el.id} value={el.id}>
        {el.name}
      </MenuItem>
    ))
  }

  return (
    <Dialog
      open={true}
      onClose={props.onClose}
      aria-labelledby="form-dialog-title"
      fullWidth
      maxWidth="sm"
    >
      <DialogTitle id="form-dialog-title">Subscribe</DialogTitle>
      <form noValidate autoComplete="off" onSubmit={formik.handleSubmit}>
        <DialogContent>
          <DialogContentText>Provide subscription data</DialogContentText>
          {renderUrlField()}
          <DialogContentText />
          {renderGroupField()}
        </DialogContent>
        <DialogActions>
          <Button type="submit">{renderSubmitButton()}</Button>
          <Button onClick={props.onClose}>Close</Button>
        </DialogActions>
      </form>
    </Dialog>
  )

  function renderUrlField() {
    return (
      <>
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
                label="Url address"
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
              />
            )
          }}
        />
        <FormHelperText error id="component-error-text">
          {formik.errors.url}
        </FormHelperText>
      </>
    )
  }

  function renderGroupField() {
    return (
      <>
        <FormControl>
          <InputLabel>Group</InputLabel>
          <Select
            id="group"
            name="group"
            onChange={event => {
              formik.handleChange(event)
            }}
            value={formik.values.group}
            displayEmpty
            onBlur={formik.handleBlur}
          >
            {renderGroupsList()}
          </Select>
        </FormControl>
        <FormHelperText error id="component-error-text">
          {formik.errors.global}
        </FormHelperText>
      </>
    )
  }
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

export default SubscribeFormPrompt
