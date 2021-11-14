import { FormControl, MenuItem, Select } from '@material-ui/core'
import { createStyles, makeStyles } from '@material-ui/core/styles'
import React from 'react'
import { useSelector } from 'react-redux'
import { blogsSlice, groupsSlice, snackbarSlice } from 'store/slices'
import { useAppDispatch } from 'store/store'

export interface BlogGroupProps {
  subId: number
  activeGroupId: number
}

export const BlogGroup: React.FC<BlogGroupProps> = props => {
  const dispatch = useAppDispatch()

  const classes = useStyles()

  const groupsList = useSelector(groupsSlice.selectAll)

  const getActiveGroup = () => {
    const group = groupsList.find(el => el.id == props.activeGroupId)
    return group != null ? group.id : -1
  }

  const assignGroup = async (groupId: number) => {
    if (groupId == props.activeGroupId) return

    const promise = await dispatch(
      blogsSlice.patchGroup({
        subId: props.subId,
        groupId: groupId,
      })
    )

    if (blogsSlice.patchGroup.fulfilled.match(promise)) {
      dispatch(
        snackbarSlice.actions.setSnackbar({
          open: true,
          color: 'success',
          msg: 'Group changed',
        })
      )
    } else {
      dispatch(
        snackbarSlice.actions.setSnackbar({
          open: true,
          color: 'error',
          msg: 'Group change failed',
        })
      )
    }
  }

  const handleChange = (event: React.ChangeEvent<{ value: unknown }>) => {
    assignGroup(event.target.value as number)
  }

  const renderGroupsList = () => {
    return groupsList.map(el => (
      <MenuItem key={el.id} value={el.id}>
        {el.name}
      </MenuItem>
    ))
  }

  return (
    <FormControl className={classes.formControl}>
      <Select
        value={getActiveGroup()}
        onChange={handleChange}
        displayEmpty
        disableUnderline
        className={classes.selectEmpty}
      >
        {renderGroupsList()}
      </Select>
    </FormControl>
  )
}

const useStyles = makeStyles(() =>
  createStyles({
    formControl: {
      minWidth: 120,
    },
    selectEmpty: {},
  })
)

export default BlogGroup
