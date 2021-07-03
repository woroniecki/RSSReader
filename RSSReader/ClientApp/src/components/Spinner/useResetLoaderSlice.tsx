import React, { useState } from 'react'
import { useAppDispatch } from 'store/store'
import { useSelector } from 'react-redux'
import { layoutSlice } from 'store/slices'
import { useHistory } from 'react-router-dom'

export const useResetLoaderSlice = () => {
  const dispatch = useAppDispatch()
  const history = useHistory()
  const { loader } = useSelector(layoutSlice.stateSelector)
  const [path, setPath] = useState('')

  React.useEffect(() => {
    setPath(location.pathname)
    return history.listen(location => {
      setPath(location.pathname)
    })
  }, [history])

  React.useEffect(() => {
    if (loader == layoutSlice.type.partial) {
      dispatch(layoutSlice.actions.setLoader(layoutSlice.type.none))
    }
  }, [path])

  return
}

export default useResetLoaderSlice
