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
import {
  Button,
  CardActions,
  CardHeader,
  CardMedia,
  Divider,
} from '@material-ui/core'

export interface ArticleCardProps {
  id: number
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
  const { id } = useParams<{ id: string }>()
  const classes = useStyles()

  const blogIdNumber = parseInt(id)

  return (
    <Card className={classes.root}>
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
        <Button onClick={() => push(`/blog/${id}/article/${props.id}`)}>
          Read
        </Button>
        <ArticlePatchButtons
          blogId={blogIdNumber}
          postId={props.id}
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
    cronerImg: {
      maxHeight: 100,
      maxWidth: 150,
    },
  })
)

export default ArticleCard
