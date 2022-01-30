import React from 'react'
import { useSelector } from 'react-redux'
import { authSlice, blogsSlice, layoutSlice } from 'store/slices'
import { useAppDispatch } from 'store/store'

export const useGetBlogsAndSubs = (): void => {
  const dispatch = useAppDispatch()

  const { token } = useSelector(authSlice.stateSelector)

  const fetchSubscribedBlogs = async () => {
    dispatch(layoutSlice.actions.setLoader(layoutSlice.type.fullScreen))

    const promise = await dispatch(blogsSlice.getSubscribedList())

    if (blogsSlice.getSubscribedList.fulfilled.match(promise)) {
    } else {
    }

    dispatch(layoutSlice.actions.setLoader(layoutSlice.type.none))
  }

  React.useEffect(() => {
    if (token) {
      fetchSubscribedBlogs()
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [token])

  return
}

export default useGetBlogsAndSubs
