import {
  createAsyncThunk,
  createEntityAdapter,
  createSlice,
  PayloadAction,
} from '@reduxjs/toolkit'
import * as blogApi from '../../api/blogApi'
import {
  Blog,
  AddSubscriptionRequest,
  PatchSubGroupRequest,
  PatchSubscriptionRequest,
} from '../../api/api.types'
import { RootState } from 'store/rootReducer'
const BLOGS = 'blogs'

export const blogsAdapter = createEntityAdapter<Blog>({
  selectId: sub => sub.id,
})

export const getSubscribedList = createAsyncThunk<
  // Return type of the payload creator
  Blog[],
  // First argument to the payload creator
  void,
  {
    rejectValue: string
  }
>(`${BLOGS}/list`, async (params, { rejectWithValue }) => {
  try {
    const res = await blogApi.getSubscribedBlogsList()
    return res
  } catch (err) {
    return rejectWithValue(err.response.data)
  }
})

export const postAddSubscription = createAsyncThunk<
  // Return type of the payload creator
  Blog,
  // First argument to the payload creator
  AddSubscriptionRequest,
  {
    rejectValue: string
  }
>(`${BLOGS}/addSubscription`, async (params, { rejectWithValue }) => {
  try {
    const res = await blogApi.postAddSubscribtions(params)
    return res
    return null
  } catch (err) {
    throw err.data
  }
})

export const putUnsubscribeBlog = createAsyncThunk<
  // Return type of the payload creator
  number,
  // First argument to the payload creator
  number,
  {
    rejectValue: string
  }
>(`${BLOGS}/unsubscribeBlog`, async (params, { rejectWithValue }) => {
  try {
    const res = await blogApi.putUnsubscribeBlog(params)
    return params
  } catch (err) {
    throw err.data
  }
})

export const patchGroup = createAsyncThunk<
  // Return type of the payload creator
  Blog,
  // First argument to the payload creator
  PatchSubGroupRequest,
  {
    rejectValue: string
  }
>(`${BLOGS}/patchGroup`, async (params, { rejectWithValue }) => {
  try {
    const res = await blogApi.patchSubscriptionGroup(
      params.subId,
      params.groupId
    )
    return res
  } catch (err) {
    throw err.data
  }
})

export const patchSubscription = createAsyncThunk<
  Blog,
  PatchSubscriptionRequest,
  {
    rejectValue: string
  }
>(`${BLOGS}/patchSubscription`, async (params, { rejectWithValue }) => {
  try {
    const res = await blogApi.patchSubscription(params)
    return res
  } catch (err) {
    return rejectWithValue(err.response.data)
  }
})

const blogsSlice = createSlice({
  name: 'blogs',
  initialState: blogsAdapter.getInitialState(),
  reducers: {
    clear: state => {
      blogsAdapter.removeAll(state)
    },
    resetGroup: (state, action: PayloadAction<number>) => {
      state.entities[action.payload].userData.groupId = -1
    },
    remove: (state, action: PayloadAction<number>) => {
      blogsAdapter.removeOne(state, action.payload)
    },
  },
  extraReducers: builder => {
    builder
      .addCase(getSubscribedList.fulfilled, (state, { payload }) => {
        blogsAdapter.setAll(state, payload)
        // state.entities[payload.id] = payload
        // both `state` and `action` are now correctly typed
        // based on the slice state and the `pending` action creator
      })
      .addCase(postAddSubscription.fulfilled, (state, { payload }) => {
        blogsAdapter.addOne(state, payload)
      })
      .addCase(putUnsubscribeBlog.fulfilled, (state, { payload }) => {
        blogsAdapter.removeOne(state, payload)
      })
      .addCase(patchGroup.fulfilled, (state, { payload }) => {
        blogsAdapter.upsertOne(state, payload)
      })
      .addCase(patchSubscription.fulfilled, (state, { payload }) => {
        blogsAdapter.upsertOne(state, payload)
      })
  },
})

export const { actions } = blogsSlice

export default blogsSlice.reducer

export const stateSelector = (state: RootState) => state.blogsReducer

export const {
  selectAll,
  selectById,
  selectIds,
} = blogsAdapter.getSelectors<RootState>(state => state.blogsReducer)
