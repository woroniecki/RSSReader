import { combineReducers } from '@reduxjs/toolkit'
import articlesReducer from './slices/articlesSlice'
import authReducer from './slices/authSlice'
import blogsReducer from './slices/blogsSlice'
import groupsReducer from './slices/groupsSlice'
import layoutSlice from './slices/layoutSlice'
import navbarSlice from './slices/navbarSlice'
import snackbarSlice from './slices/snackbarSlice'

export const rootReducer = combineReducers({
  authReducer,
  layoutSlice,
  blogsReducer,
  articlesReducer,
  groupsReducer,
  navbarSlice,
  snackbarSlice,
})

export type RootState = ReturnType<typeof rootReducer>
