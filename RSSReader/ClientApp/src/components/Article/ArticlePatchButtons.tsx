import React from 'react'
import { useHistory } from 'react-router-dom'
import { useAppDispatch } from 'store/store'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faStar } from '@fortawesome/free-solid-svg-icons'
import { faBook } from '@fortawesome/free-solid-svg-icons'
import { faBookOpen } from '@fortawesome/free-solid-svg-icons'
import { Button } from 'react-bootstrap'
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
      <Button
        onClick={() => {
          patchPost(props.blogId, props.postId, null, !props.favourite)
        }}
      >
        <FontAwesomeIcon
          color={props.favourite ? 'yellow' : 'white'}
          icon={faStar}
        />
      </Button>
      <Button
        onClick={() => {
          patchPost(props.blogId, props.postId, !props.readed, null)
        }}
      >
        <FontAwesomeIcon icon={props.readed ? faBookOpen : faBook} />
      </Button>
    </React.Fragment>
  )
}

export default ArticlePatchButtons
