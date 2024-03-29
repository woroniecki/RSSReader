import React from 'react'
import { useSelector } from 'react-redux'
import { useParams } from 'react-router-dom'
import { articlesSlice, authSlice, blogsSlice, layoutSlice } from 'store/slices'
import { useAppDispatch } from 'store/store'

export const useGetArticles = () => {
  const dispatch = useAppDispatch()
  const { blogid } = useParams<{ blogid: string }>()
  const { token } = useSelector(authSlice.stateSelector)
  const blogsList = useSelector(blogsSlice.selectAll)
  const articlesList = useSelector(articlesSlice.selectAll)

  const fetchList = async (): Promise<void> => {
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
  }, [token])

  const url = window.location.pathname.split('/').pop()
  React.useEffect(() => {
    if (token) {
      fetchList()
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [url])

  return
}

export default useGetArticles
