import { IconButton } from '@material-ui/core'
import { yellow } from '@material-ui/core/colors'
import { createStyles, makeStyles } from '@material-ui/core/styles'
import ImportContactsIcon from '@material-ui/icons/ImportContacts'
import MenuBookIcon from '@material-ui/icons/MenuBook'
import StarIcon from '@material-ui/icons/Star'
import { PostUserData } from 'api/api.types'
import React from 'react'
import { articlesSlice } from 'store/slices'
import { useAppDispatch } from 'store/store'

export interface ArticlePatchButtonsProps {
  blogId: number
  postId: number
  userData: PostUserData
}

export const ArticlePatchButtons: React.FC<ArticlePatchButtonsProps> = props => {
  const dispatch = useAppDispatch()
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

  if (props.userData == null) return <></>

  return (
    <React.Fragment>
      <IconButton
        aria-label="favourite"
        className={props.userData.favourite ? classes.starYellow : classes.star}
        onClick={() => {
          patchPost(props.blogId, props.postId, null, !props.userData.favourite)
        }}
      >
        <StarIcon />
      </IconButton>
      <IconButton
        aria-label="readed"
        onClick={() => {
          patchPost(props.blogId, props.postId, !props.userData.readed, null)
        }}
      >
        {props.userData.readed ? <ImportContactsIcon /> : <MenuBookIcon />}
      </IconButton>
    </React.Fragment>
  )
}

const useStyles = makeStyles(() =>
  createStyles({
    starYellow: {
      color: yellow[500],
    },
    star: {},
  })
)

export default ArticlePatchButtons
