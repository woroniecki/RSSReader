import React, { useState } from 'react'
import { useSelector } from 'react-redux'
import { useHistory, useParams } from 'react-router-dom'
import { blogsSlice, articlesSlice, authSlice } from 'store/slices'
import { useAppDispatch } from 'store/store'
import { layoutSlice } from 'store/slices'

export const useGetArticles = () => {
  const dispatch = useAppDispatch()
  const { push } = useHistory()
  const { blogid } = useParams<{ blogid: string }>()
  const { token } = useSelector(authSlice.stateSelector)
  const blogsList = useSelector(blogsSlice.selectAll)
  const articlesList = useSelector(articlesSlice.selectAll)
  const [isFiltering, setFiltering] = useState(false)

  const fetchList = async () => {
    const blogId = parseInt(blogid)
    if (blogId == NaN) return

    const list_already_taken = articlesList.find(el => el.blogId == blogId)
    if (list_already_taken != null) return

    dispatch(layoutSlice.actions.setLoader(layoutSlice.type.partial))

    const promise = await dispatch(articlesSlice.getArticles(blogId))

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
  }, [blogsList])

  return
}

export default useGetArticles
