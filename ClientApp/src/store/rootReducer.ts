import { combineReducers } from '@reduxjs/toolkit'
import authReducer from './slices/authSlice'
import layoutSlice from './slices/layoutSlice'
import subscriptionsReducer from './slices/subscriptionsSlice'
import articlesReducer from './slices/articlesSlice'
import groupsReducer from './slices/groupsSlice'

export const rootReducer = combineReducers({
  authReducer,
  layoutSlice,
  subscriptionsReducer,
  articlesReducer,
  groupsReducer,
})

export type RootState = ReturnType<typeof rootReducer>
