import { Avatar, Typography } from '@material-ui/core'
import React from 'react'
import { useHistory } from 'react-router-dom'
import { useAppDispatch } from 'store/store'
import { makeStyles, Theme, createStyles } from '@material-ui/core/styles'
import { red } from '@material-ui/core/colors'

export type Size = 'small' | 'large'

export interface BlogAvatarProps {
  title: string
  imageUrl: string
  size?: Size
}

export const BlogAvatar: React.FC<BlogAvatarProps> = props => {
  const classes = useStyles()

  function getAvatarClass() {
    if (props.size === 'small') return classes.avatarSmall
    return classes.avatarLarge
  }

  return (
    <Avatar
      aria-label="recipe"
      className={getAvatarClass()}
      src={props.imageUrl}
    >
      <Typography variant="h5">
        {props.title.length > 0 && props.title.charAt(0)}
      </Typography>
    </Avatar>
  )
}

const useStyles = makeStyles((theme: Theme) =>
  createStyles({
    avatarSmall: {
      backgroundColor: red[500],
      width: theme.spacing(3),
      height: theme.spacing(3),
    },
    avatarLarge: {
      backgroundColor: red[500],
      width: theme.spacing(7),
      height: theme.spacing(7),
    },
  })
)

export default BlogAvatar
