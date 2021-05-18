import React from 'react'
import { useAppDispatch } from 'store/store'
import { authSlice, subscriptionsSlice } from 'store/slices'
import { useSelector } from 'react-redux'

export const useGetBlogsAndSubs = () => {
  const dispatch = useAppDispatch()

  const { token } = useSelector(authSlice.stateSelector)

  const fetchList = async () => {
    const promise = await dispatch(subscriptionsSlice.getList())

    if (subscriptionsSlice.getList.fulfilled.match(promise)) {
    } else {
    }
  }

  React.useEffect(() => {
    if (token) {
      fetchList()
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [token])

  return
}

export default useGetBlogsAndSubs
