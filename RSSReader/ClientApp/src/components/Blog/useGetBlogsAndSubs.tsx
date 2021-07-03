import React from 'react'
import { useAppDispatch } from 'store/store'
import { authSlice, blogsSlice } from 'store/slices'
import { useSelector } from 'react-redux'
import { layoutSlice } from 'store/slices'

export const useGetBlogsAndSubs = (): void => {
  const dispatch = useAppDispatch()

  const { token } = useSelector(authSlice.stateSelector)

  const fetchSubscribedBlogs = async () => {
    dispatch(layoutSlice.actions.setLoader(layoutSlice.type.partial))

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
