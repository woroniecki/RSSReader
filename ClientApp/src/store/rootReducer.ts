import { combineReducers } from '@reduxjs/toolkit'
import authReducer from './slices/authSlice'
import layoutSlice from './slices/layoutSlice'
import subscriptionsReducer from './slices/subscriptionsSlice'
import articlesReducer from './slices/articlesSlice'

export const rootReducer = combineReducers({
  authReducer,
  layoutSlice,
  subscriptionsReducer,
  articlesReducer,
})

export type RootState = ReturnType<typeof rootReducer>
