import React, { useState } from 'react'
import { Button } from 'react-bootstrap'
import { useSelector } from 'react-redux'
import { useHistory, useParams } from 'react-router-dom'
import { subscriptionsSlice, articlesSlice, authSlice } from 'store/slices'
import { useAppDispatch } from 'store/store'
import { layoutSlice } from 'store/slices'

export const useGetArticles = () => {
  const dispatch = useAppDispatch()
  const { push } = useHistory()
  const { id } = useParams<{ id: string }>()
  const { token } = useSelector(authSlice.stateSelector)
  const subscriptionsList = useSelector(subscriptionsSlice.selectAll)
  const articlesList = useSelector(articlesSlice.selectAll)
  const [isFiltering, setFiltering] = useState(false)

  const getCurrentBlogId = () => {
    const subId = parseInt(id)
    if (subId == NaN) return -1

    const sub = subscriptionsList.find(el => el.id == subId)
    if (sub == null) return -1

    return sub.blog.id
  }

  const fetchList = async () => {
    const blogid = getCurrentBlogId()
    if (blogid < 0) return

    const list_already_taken = articlesList.find(el => el.blogId == blogid)
    if (list_already_taken != null) return

    dispatch(layoutSlice.actions.setLoader(layoutSlice.type.partial))

    const promise = await dispatch(articlesSlice.getArticles(blogid))

    if (articlesSlice.getArticles.fulfilled.match(promise)) {
    } else {
    }

    dispatch(layoutSlice.actions.setLoader(layoutSlice.type.none))
  }
  React.useEffect(() => {
    if (token) {
      fetchList()
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [subscriptionsList])

  return
}

export default useGetArticles
