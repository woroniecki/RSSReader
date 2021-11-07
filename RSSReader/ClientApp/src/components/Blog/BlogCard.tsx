import React from 'react'
import { makeStyles, Theme, createStyles } from '@material-ui/core/styles'
import Card from '@material-ui/core/Card'
import CardContent from '@material-ui/core/CardContent'
import Typography from '@material-ui/core/Typography'
import { useHistory } from 'react-router-dom'
import { useAppDispatch } from 'store/store'
import { blogsSlice, articlesSlice } from 'store/slices'
import { useSelector } from 'react-redux'

import { BlogGroup } from './BlogGroup'
import UnsubscribeBlogBtn from './UnsubscribeBlogBtn'
import { Button, CardActions, CardHeader, Divider } from '@material-ui/core'
import BlogAvatar from './BlogAvatar'
import { Blog } from 'api/api.types'

export interface BlogCardProps {
  blog: Blog
}

export const BlogCard: React.FC<BlogCardProps> = props => {
  const { push } = useHistory()

  const classes = useStyles()

  const blogsList = useSelector(blogsSlice.selectAll)
  const articlesList = useSelector(articlesSlice.selectAll)

  function DrawUnreadedAmount() {
    if (props.blog == null) return

    if (props.blog.userData == null) return

    let amount = articlesList.filter(
      el =>
        el.blogId == props.blog.id && el.userData != null && !el.userData.readed
    ).length

    if (amount <= 0 && props.blog.userData.unreadedCount != null) {
      amount = props.blog.userData.unreadedCount
    }

    if (amount <= 0) return

    return `New ` + amount.toString() + (amount >= 10 ? `+` : ` `)
  }

  function DrawGroup() {
    if (props.blog == null) return

    if (props.blog.userData == null) return

    return (
      <BlogGroup
        subId={props.blog.userData.subId}
        activeGroupId={props.blog.userData.groupId}
      />
    )
  }

  return (
    <Card className={classes.root}>
      <CardHeader
        onClick={() => push(`/blog/${props.blog.id}`)}
        avatar={
          <BlogAvatar title={props.blog.name} imageUrl={props.blog.imageUrl} />
        }
        action={<>{DrawUnreadedAmount()}</>}
        title={<Typography variant="h5">{props.blog.name}</Typography>}
        subheader={props.blog.description}
      />
      <Divider />
      <CardActions disableSpacing>
        <Button onClick={() => push(`/blog/${props.blog.id}`)}>Read</Button>
        <UnsubscribeBlogBtn blog={props.blog} />
      </CardActions>
    </Card>
  )
}

const useStyles = makeStyles(() =>
  createStyles({
    root: {
      maxWidth: 845,
      margin: '15px 5px',
    },
    media: {
      height: 0,
      paddingTop: '56.25%', // 16:9
    },
  })
)

export default BlogCard
