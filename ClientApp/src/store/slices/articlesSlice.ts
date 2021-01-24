import {
  createAsyncThunk,
  createEntityAdapter,
  createSlice,
} from '@reduxjs/toolkit'
import * as blogApi from '../../api/blogApi'
import { Post } from '../../api/api.types'
import { RootState } from 'store/rootReducer'
const ARTICLES = 'articles'

export const articlesAdapter = createEntityAdapter<Post>({
  selectId: sub => sub.id,
})

export const getArticles = createAsyncThunk<
  // Return type of the payload creator
  Post[],
  // First argument to the payload creator
  number,
  {
    rejectValue: string
  }
>(`${ARTICLES}/list`, async (blogid, { rejectWithValue }) => {
  try {
    const res = await blogApi.getPostsList(blogid)
    let id = 0
    res.map(el => (el.id = id++))
    return res
  } catch (err) {
    return rejectWithValue(err.response.data)
  }
})

const articlesSlice = createSlice({
  name: 'articles',
  initialState: articlesAdapter.getInitialState(),
  reducers: {},
  extraReducers: builder => {
    builder.addCase(getArticles.fulfilled, (state, { payload }) => {
      articlesAdapter.setAll(state, payload)
    })
  },
})

export const { actions } = articlesSlice

export default articlesSlice.reducer

export const stateSelector = (state: RootState) => state.articlesReducer

export const {
  selectAll,
  selectById,
  selectIds,
} = articlesAdapter.getSelectors<RootState>(state => state.articlesReducer)
