import React from 'react'
import { makeStyles, Theme, createStyles } from '@material-ui/core/styles'
import Card from '@material-ui/core/Card'
import CardContent from '@material-ui/core/CardContent'
import { useHistory, useParams } from 'react-router-dom'
import { useAppDispatch } from 'store/store'
import Typography from '@material-ui/core/Typography'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faStar } from '@fortawesome/free-solid-svg-icons'
import { faBook } from '@fortawesome/free-solid-svg-icons'
import { faBookOpen } from '@fortawesome/free-solid-svg-icons'
import { articlesSlice } from 'store/slices'
import ArticlePatchButtons from './ArticlePatchButtons'
import { Button, CardActions, CardHeader, Divider } from '@material-ui/core'

export interface ArticleCardProps {
  articleid: number
  title: string
  summary: string
  content: string
  url: string
  imageUrl: string
  readed: boolean
  favourite: boolean
  publishDate: string
}

export const ArticleCard: React.FC<ArticleCardProps> = props => {
  const dispatch = useAppDispatch()
  const { push } = useHistory()
  const { blogid } = useParams<{ blogid: string }>()
  const classes = useStyles()

  const blogId = parseInt(blogid)
  if (blogId == NaN) return

  return (
    <Card className={props.readed ? classes.rootTransparent : classes.root}>
      <CardHeader
        title={<Typography variant="h5">{props.title}</Typography>}
        subheader={
          <time dateTime={props.publishDate}>{props.publishDate}</time>
        }
        action={<img className={classes.cronerImg} src={props.imageUrl} />}
      />
      <Divider />
      <CardContent>
        <Typography variant="body2" color="textSecondary" component="p">
          {props.summary}
        </Typography>
      </CardContent>
      <CardActions disableSpacing>
        <Button
          onClick={() => push(`/blog/${blogid}/article/${props.articleid}`)}
        >
          Read
        </Button>
        <ArticlePatchButtons
          blogId={blogId}
          postId={props.articleid}
          readed={props.readed}
          favourite={props.favourite}
        />
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
    rootTransparent: {
      maxWidth: 845,
      margin: '15px 5px',
      opacity: 0.7,
    },
    cronerImg: {
      maxHeight: 100,
      maxWidth: 150,
    },
  })
)

export default ArticleCard
