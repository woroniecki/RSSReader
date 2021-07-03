import React from 'react'
import { makeStyles, Theme, createStyles } from '@material-ui/core/styles'
import Card from '@material-ui/core/Card'
import CardContent from '@material-ui/core/CardContent'
import Typography from '@material-ui/core/Typography'
import { Link, useHistory } from 'react-router-dom'
import { useAppDispatch } from 'store/store'
import { blogsSlice, articlesSlice } from 'store/slices'
import { useSelector } from 'react-redux'

import { BlogGroup } from './BlogGroup'
import UnsubscribeBlogBtn from './UnsubscribeBlogBtn'
import {
  Avatar,
  Button,
  CardActions,
  CardHeader,
  Divider,
  List,
  ListItem,
} from '@material-ui/core'
import BlogAvatar from './BlogAvatar'

export interface BlogCardProps {
  title: string
  description: string
  imageUrl: string
  blogid: number
}

export const BlogCard: React.FC<BlogCardProps> = props => {
  const dispatch = useAppDispatch()
  const { push } = useHistory()

  const classes = useStyles()

  const blogsList = useSelector(blogsSlice.selectAll)
  const articlesList = useSelector(articlesSlice.selectAll)

  const updateArticlesList = async () => {
    const list_already_taken = articlesList.find(
      el => el.blogId == props.blogid
    )
    if (list_already_taken != null) return

    const promise = await dispatch(articlesSlice.getArticles(props.blogid))

    if (articlesSlice.getArticles.fulfilled.match(promise)) {
    } else {
    }
  }

  function DrawUnreadedAmount() {
    const blog = blogsList.find(el => el.id == props.blogid)

    if (blog == null) return

    if (blog.userData == null) return

    let amount = articlesList.filter(
      el => el.blogId == props.blogid && !el.readed
    ).length

    if (amount <= 0 && blog.userData.unreadedCount != null) {
      amount = blog.userData.unreadedCount
    }

    if (amount <= 0) return

    return `New ` + amount.toString() + (amount >= 10 ? `+` : ` `)
  }

  function DrawGroup() {
    const blog = blogsList.find(el => el.id == props.blogid)

    if (blog == null) return

    if (blog.userData == null) return

    return (
      <BlogGroup subId={props.blogid} activeGroupId={blog.userData.groupId} />
    )
  }

  const no_blog_img_url =
    'https://www.pngfind.com/pngs/m/269-2693798_png-file-svg-blog-vector-icon-png-transparent.png'

  return (
    <Card className={classes.root}>
      <CardHeader
        avatar={<BlogAvatar title={props.title} imageUrl={props.imageUrl} />}
        action={
          <>
            {DrawUnreadedAmount()}
            <UnsubscribeBlogBtn id={props.blogid} />
          </>
        }
        title={<Typography variant="h5">{props.title}</Typography>}
        subheader={DrawGroup()}
      />
      <Divider />
      <CardContent>
        <Typography variant="body2" color="textSecondary" component="p">
          {props.description}
        </Typography>
      </CardContent>
      <CardActions disableSpacing>
        <Button onClick={() => push(`/blog/${props.blogid}`)}>Read</Button>
      </CardActions>
    </Card>
  )
}

const useStyles = makeStyles((theme: Theme) =>
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
