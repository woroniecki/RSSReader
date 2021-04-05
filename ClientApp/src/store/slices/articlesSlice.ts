import {
  createAsyncThunk,
  createEntityAdapter,
  createSlice,
} from '@reduxjs/toolkit'
import * as blogApi from '../../api/blogApi'
import { Post, ReadPostRequest } from '../../api/api.types'
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
    res.map(el => {
      el.blogId = blogid
    })
    return res
  } catch (err) {
    return rejectWithValue(err.response.data)
  }
})

export const putReadArticle = createAsyncThunk<
  Post,
  ReadPostRequest,
  {
    rejectValue: string
  }
>(`${ARTICLES}/read`, async (params, { rejectWithValue }) => {
  try {
    const res = await blogApi.putReadPost(params.blogId, params.postId)
    res.blogId = params.blogId
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
    builder
      .addCase(getArticles.fulfilled, (state, { payload }) => {
        articlesAdapter.setAll(state, payload)
      })
      .addCase(putReadArticle.fulfilled, (state, { payload }) => {
        articlesAdapter.removeOne(state, payload.id)
        articlesAdapter.addOne(state, payload)
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
