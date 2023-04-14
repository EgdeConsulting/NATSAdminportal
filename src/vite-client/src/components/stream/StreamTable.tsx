import { useEffect, useState } from "react";
import {
  PaginatedTable,
  StreamViewButton,
  LoadingSpinner,
  SelectColumnFilter,
  IStream,
} from "components";

function StreamTable() {
  /**
   * This constant defines the configuration of the paginated table.
   * The “accessor” property is equal to a property contained in a data object.
   * More information at: https://tanstack.com/table/v8/docs/guide/column-defs
   */
  const columns = [
    {
      Header: "StreamTable",
      columns: [
        {
          Header: "Name",
          accessor: "name",
          disableFilters: true,
        },
        {
          Header: "No. Subjects",
          accessor: "subjectCount",
          Filter: SelectColumnFilter,
          filter: "equals",
        },
        {
          Header: "No. Consumers",
          accessor: "consumerCount",
          Filter: SelectColumnFilter,
          filter: "equals",
        },
        {
          Header: "No. Messages",
          accessor: "messageCount",
          disableFilters: true,
        },
        {
          Header: "Data",
          accessor: "data",
          disableFilters: true,
          Cell: (props: { row: any }) => {
            return <StreamViewButton content={props.row.values} />;
          },
        },
      ],
    },
  ];

  const [streams, setStreams] = useState<IStream[]>([]);
  const [isIntervalRunning, setIsIntervalRunning] = useState(false);
  const [loading, setLoading] = useState(true);

  function getStreams() {
    fetch("/api/allStreams").then((res) => {
      if (res.ok) {
        res.json().then((data: IStream[]) => {
          setStreams(data);
          setLoading(false);
        });
      } else {
        alert(
          "An error occurred while fetching all streams: " + res.statusText
        );
      }
    });
  }

  /**
   * Starting an interval which frequently send new API-requests to get
   * the newest data. Only one interval is being started via the use of
   * the “isIntervalRunning” state. The interval is also being discarded
   * as soon as the component dismounts.
   */
  useEffect(() => {
    setIsIntervalRunning(true);
    const interval = setInterval(getStreams, 5000);
    return () => {
      clearInterval(interval);
      setIsIntervalRunning(false);
    };
  }, [!isIntervalRunning]);

  return (
    <>
      {loading ? (
        <LoadingSpinner />
      ) : (
        <PaginatedTable columns={columns} data={streams} />
      )}
    </>
  );
}
export { StreamTable };
