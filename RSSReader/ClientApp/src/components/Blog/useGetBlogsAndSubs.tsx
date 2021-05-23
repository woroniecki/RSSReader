import React from 'react'
import { useAppDispatch } from 'store/store'
import { authSlice, subscriptionsSlice } from 'store/slices'
import { useSelector } from 'react-redux'
import { layoutSlice } from 'store/slices'

export const useGetBlogsAndSubs = () => {
  const dispatch = useAppDispatch()

  const { token } = useSelector(authSlice.stateSelector)

  const fetchList = async () => {
    dispatch(layoutSlice.actions.setLoader(layoutSlice.type.partial))

    const promise = await dispatch(subscriptionsSlice.getList())

    if (subscriptionsSlice.getList.fulfilled.match(promise)) {
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

  return
}

export default useGetBlogsAndSubs
