import { Button, CardActions, CardHeader, Divider } from '@material-ui/core'
import Card from '@material-ui/core/Card'
import CardContent from '@material-ui/core/CardContent'
import { createStyles, makeStyles } from '@material-ui/core/styles'
import Typography from '@material-ui/core/Typography'
import { PostUserData } from 'api/api.types'
import AppTypography from 'components/layout/AppTypography'
import React from 'react'
import { useHistory, useParams } from 'react-router-dom'
import { formatDate } from 'utils/utils'
import ArticlePatchButtons from './ArticlePatchButtons'

export interface ArticleCardProps {
  articleid: number
  title: string
  summary: string
  content: string
  url: string
  imageUrl: string
  publishDate: string
  userData: PostUserData
}

export const ArticleCard: React.FC<ArticleCardProps> = props => {
  const { push } = useHistory()
  const { blogid } = useParams<{ blogid: string }>()
  const classes = useStyles()

  const blogId = parseInt(blogid)
  if (blogId == NaN) return

  return (
    <Card
      className={
        props.userData != null && props.userData.readed
          ? classes.rootTransparent
          : classes.root
      }
    >
      <CardHeader
        title={<AppTypography variant="h5">{props.title}</AppTypography>}
        subheader={
          <time dateTime={props.publishDate}>
            {formatDate(props.publishDate)}
          </time>
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
          userData={props.userData}
        />
      </CardActions>
    </Card>
  )
}

const useStyles = makeStyles(() =>
  createStyles({
    root: {
      margin: '15px 5px',
    },
    rootTransparent: {
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
