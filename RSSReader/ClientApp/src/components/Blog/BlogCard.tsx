import React from 'react'
import { makeStyles, Theme, createStyles } from '@material-ui/core/styles'
import Card from '@material-ui/core/Card'
import CardContent from '@material-ui/core/CardContent'
import CardMedia from '@material-ui/core/CardMedia'
import IconButton from '@material-ui/core/IconButton'
import Typography from '@material-ui/core/Typography'
import { red } from '@material-ui/core/colors'
import { Link, useHistory } from 'react-router-dom'
import { useAppDispatch } from 'store/store'
import { subscriptionsSlice, articlesSlice } from 'store/slices'
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

export interface BlogCardProps {
  title: string
  description: string
  imageUrl: string
  id: number
}

export const BlogCard: React.FC<BlogCardProps> = props => {
  const dispatch = useAppDispatch()
  const { push } = useHistory()

  const classes = useStyles()

  const subscriptionsList = useSelector(subscriptionsSlice.selectAll)
  const articlesList = useSelector(articlesSlice.selectAll)

  const updateArticlesList = async () => {
    const list_already_taken = articlesList.find(el => el.blogId == props.id)
    if (list_already_taken != null) return

    const promise = await dispatch(articlesSlice.getArticles(props.id))

    if (articlesSlice.getArticles.fulfilled.match(promise)) {
    } else {
    }
  }

  function getUnreadedAmount() {
    const sub = subscriptionsList.find(el => el.id == props.id)

    if (sub == null) return

    let amount = articlesList.filter(el => el.blogId == props.id && !el.readed)
      .length

    if (amount <= 0 && sub.unreadedCount != null) {
      amount = sub.unreadedCount
    }

    if (amount <= 0) return

    return `New ` + amount.toString() + (amount >= 10 ? `+` : ` `)
  }

  function getGroupId() {
    const sub = subscriptionsList.find(el => el.id == props.id)

    return sub != null ? sub.groupId : -1
  }

  const no_blog_img_url =
    'https://www.pngfind.com/pngs/m/269-2693798_png-file-svg-blog-vector-icon-png-transparent.png'

  return (
    <Card className={classes.root}>
      <CardHeader
        avatar={
          <Avatar
            aria-label="recipe"
            className={classes.avatar}
            src={props.imageUrl}
          >
            <Typography variant="h5">
              {props.title.length > 0 && props.title.charAt(0)}
            </Typography>
          </Avatar>
        }
        action={
          <>
            {getUnreadedAmount()}
            <UnsubscribeBlogBtn id={props.id} />
          </>
        }
        title={<Typography variant="h5">{props.title}</Typography>}
        subheader={<BlogGroup subId={props.id} activeGroupId={getGroupId()} />}
      />
      <Divider />
      <CardContent>
        <Typography variant="body2" color="textSecondary" component="p">
          {props.description}
        </Typography>
      </CardContent>
      <CardActions disableSpacing>
        <Button onClick={() => push(`/blog/${props.id}`)}>Read</Button>
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
    expand: {
      transform: 'rotate(0deg)',
      marginLeft: 'auto',
      transition: theme.transitions.create('transform', {
        duration: theme.transitions.duration.shortest,
      }),
    },
    expandOpen: {
      transform: 'rotate(180deg)',
    },
    avatar: {
      backgroundColor: red[500],
      width: theme.spacing(7),
      height: theme.spacing(7),
    },
  })
)

export default BlogCard
