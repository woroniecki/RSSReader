import { Avatar, Typography } from '@material-ui/core'
import React from 'react'
import { useHistory } from 'react-router-dom'
import { useAppDispatch } from 'store/store'
import { makeStyles, Theme, createStyles } from '@material-ui/core/styles'
import { red } from '@material-ui/core/colors'
import randomColor from 'randomcolor'

export type Size = 'small' | 'large'

export interface BlogAvatarProps {
  title: string
  imageUrl: string
  size?: Size
  variant?: 'circle' | 'circular' | 'rounded' | 'square'
}

export const BlogAvatar: React.FC<BlogAvatarProps> = props => {
  const classes = useStyles()

  function getAvatarClass() {
    if (props.size === 'small') return classes.avatarSmall
    return classes.avatarLarge
  }

  function color(seed: string) {
    return randomColor({
      luminosity: 'bright',
      seed: seed,
    })
  }

  return (
    <Avatar
      aria-label="recipe"
      variant={props.variant}
      style={{ backgroundColor: color(props.title) }}
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
      margin: '0px 10px 0px 12px',
    },
    avatarLarge: {
      backgroundColor: red[500],
      width: theme.spacing(6),
      height: theme.spacing(6),
    },
  })
)

export default BlogAvatar
