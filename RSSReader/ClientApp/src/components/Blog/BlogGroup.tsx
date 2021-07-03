import React from 'react'
import { createStyles, makeStyles, Theme } from '@material-ui/core/styles'
import { useHistory, useParams } from 'react-router-dom'
import { useAppDispatch } from 'store/store'
import { groupsSlice, blogsSlice } from 'store/slices'
import { useSelector } from 'react-redux'
import { FormControl, MenuItem, Select } from '@material-ui/core'

export interface BlogGroupProps {
  subId: number
  activeGroupId: number
}

export const BlogGroup: React.FC<BlogGroupProps> = props => {
  const dispatch = useAppDispatch()
  const { push } = useHistory()

  const classes = useStyles()

  const groupsList = useSelector(groupsSlice.selectAll)
  const none_group = 'None'

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

    if (groupsSlice.remove.fulfilled.match(promise)) {
    } else {
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

const useStyles = makeStyles((theme: Theme) =>
  createStyles({
    formControl: {
      minWidth: 120,
    },
    selectEmpty: {},
  })
)

export default BlogGroup
