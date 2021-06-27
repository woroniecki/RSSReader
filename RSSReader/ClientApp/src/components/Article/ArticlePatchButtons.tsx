import React from 'react'
import { useHistory } from 'react-router-dom'
import { makeStyles, Theme, createStyles } from '@material-ui/core/styles'
import { useAppDispatch } from 'store/store'
import StarIcon from '@material-ui/icons/Star'
import MenuBookIcon from '@material-ui/icons/MenuBook'
import ImportContactsIcon from '@material-ui/icons/ImportContacts'
import { IconButton } from '@material-ui/core'
import { yellow } from '@material-ui/core/colors'
import { articlesSlice } from 'store/slices'

export interface ArticlePatchButtonsProps {
  blogId: number
  postId: number
  favourite: boolean
  readed: boolean
}

export const ArticlePatchButtons: React.FC<ArticlePatchButtonsProps> = props => {
  const dispatch = useAppDispatch()
  const { push } = useHistory()
  const classes = useStyles()

  const patchPost = async (
    blogId: number,
    postId: number,
    readed: boolean,
    favourite: boolean
  ) => {
    const promise = await dispatch(
      articlesSlice.patchPost({
        blogId: blogId,
        postId: postId,
        readed: readed,
        favourite: favourite,
      })
    )

    if (articlesSlice.patchPost.fulfilled.match(promise)) {
    } else {
    }
  }

  return (
    <React.Fragment>
      <IconButton
        aria-label="favourite"
        className={props.favourite ? classes.starYellow : classes.star}
        onClick={() => {
          patchPost(props.blogId, props.postId, null, !props.favourite)
        }}
      >
        <StarIcon />
      </IconButton>
      <IconButton
        aria-label="readed"
        onClick={() => {
          patchPost(props.blogId, props.postId, !props.readed, null)
        }}
      >
        {props.readed ? <ImportContactsIcon /> : <MenuBookIcon />}
      </IconButton>
    </React.Fragment>
  )
}

const useStyles = makeStyles((theme: Theme) =>
  createStyles({
    starYellow: {
      color: yellow[500],
    },
    star: {},
  })
)

export default ArticlePatchButtons
