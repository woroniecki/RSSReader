import { Button, CardActions, CardHeader, Divider } from '@material-ui/core'
import Card from '@material-ui/core/Card'
import { createStyles, makeStyles } from '@material-ui/core/styles'
import Typography from '@material-ui/core/Typography'
import SubjectOutlinedIcon from '@material-ui/icons/SubjectOutlined'
import { Blog } from 'api/api.types'
import AppTypography from 'components/layout/AppTypography'
import React from 'react'
import { useSelector } from 'react-redux'
import { useHistory, useParams } from 'react-router-dom'
import { articlesSlice, blogsSlice } from 'store/slices'
import BlogAvatar from './BlogAvatar'
import { BlogGroup } from './BlogGroup'
import UnsubscribeBlogBtn from './UnsubscribeBlogBtn'
import { getUrlWithGroupId } from 'utils/utils'

export interface BlogCardProps {
  blog: Blog
}

export const BlogCard: React.FC<BlogCardProps> = props => {
  const { push } = useHistory()

  const classes = useStyles()

  const blogsList = useSelector(blogsSlice.selectAll)
  const articlesList = useSelector(articlesSlice.selectAll)

  const { groupId } = useParams<{ groupId: string }>()

  function DrawUnreadedAmount() {
    if (props.blog == null) return

    if (props.blog.userData == null) return

    const all_posts_amount = articlesList.filter(
      el =>
        el.blogId == props.blog.id
    ).length

    let amount_unreaded = articlesList.filter(
      el =>
        el.blogId == props.blog.id && el.userData != null && !el.userData.readed
    ).length

    if (all_posts_amount <= 0 && props.blog.userData.unreadedCount != null) {
      amount_unreaded = props.blog.userData.unreadedCount
    }

    if (amount_unreaded <= 0) return

    return `New ` + amount_unreaded.toString() + `+`
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
        onClick={() => push(getUrlWithGroupId(`/blog/${props.blog.id}`, groupId))}
        avatar={
          <BlogAvatar title={props.blog.name} imageUrl={props.blog.imageUrl} />
        }
        action={<Typography noWrap>{DrawUnreadedAmount()}</Typography>}
        title={<AppTypography variant="h5">{props.blog.name}</AppTypography>}
        subheader={<AppTypography>{props.blog.description}</AppTypography>}
      />
      <Divider />
      <CardActions disableSpacing>
        <Button onClick={() => push(getUrlWithGroupId(`/blog/${props.blog.id}`, groupId))}>
          <SubjectOutlinedIcon />
          Read
        </Button>
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
