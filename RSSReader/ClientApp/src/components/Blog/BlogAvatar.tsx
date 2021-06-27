import { Avatar, Typography } from '@material-ui/core'
import React from 'react'
import { useHistory } from 'react-router-dom'
import { useAppDispatch } from 'store/store'
import { makeStyles, Theme, createStyles } from '@material-ui/core/styles'
import { red } from '@material-ui/core/colors'

export interface BlogAvatarProps {
  title: string
  imageUrl: string
}

export const BlogAvatar: React.FC<BlogAvatarProps> = props => {
  const dispatch = useAppDispatch()
  const { push } = useHistory()
  const classes = useStyles()

  return (
    <Avatar aria-label="recipe" className={classes.avatar} src={props.imageUrl}>
      <Typography variant="h5">
        {props.title.length > 0 && props.title.charAt(0)}
      </Typography>
    </Avatar>
  )
}

const useStyles = makeStyles((theme: Theme) =>
  createStyles({
    avatar: {
      backgroundColor: red[500],
      width: theme.spacing(7),
      height: theme.spacing(7),
    },
  })
)

export default BlogAvatar
